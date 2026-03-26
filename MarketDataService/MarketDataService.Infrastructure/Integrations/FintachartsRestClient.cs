using MarketDataService.Application.DTOs;
using MarketDataService.Application.Interfaces;
using MarketDataService.Domain.Entities;
using MarketDataService.Infrastructure.Configuration;
using MarketDataService.Infrastructure.Models;
using MarketDataService.Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MarketDataService.Infrastructure.Integrations
{
    public class FintachartsRestClient : IFintachartsRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly FintachartsAuthService  _authService;
        private readonly FintachartsSettings _settings;

        public FintachartsRestClient(HttpClient httpClient, FintachartsAuthService authService, IOptions<FintachartsSettings> options)
        {
            _httpClient = httpClient;
            _authService = authService;
            _settings = options.Value;
        }
        public async Task<IEnumerable<Asset>> GetAssetsAsync(string provider, CancellationToken cancellationToken)
        {

            var url = $"/api/instruments/v1/instruments?provider={provider}&kind=forex";

            var response = await SendAuthorizedGetAsync<PagedResponse<InstrumentDto>>(url, cancellationToken);

            if (response?.Data == null) return Enumerable.Empty<Asset>();

            return response.Data.Select(dto => new Asset
            {
                Id = dto.Id,
                Symbol = dto.Symbol,
                Description = dto.Description,
                Providers = dto.Mappings.Keys.ToList()
            }).ToList();
        }

        public async Task<HistoricalPriceResponse?> GetLastHistoricalPriceAsync(HistoricalPriceRequest request, CancellationToken cancellationToken)
        {
            var url = $"/api/bars/v1/bars/count-back?instrumentId={request.InstrumentId}&provider={request.Provider}&interval=1&periodicity=minute&barsCount=1";

            var response = await SendAuthorizedGetAsync<PagedResponse<HistoricalBarDto>>(url, cancellationToken);

            var lastBar = response?.Data?.FirstOrDefault();

            if (lastBar == null) return null;

            return new HistoricalPriceResponse(lastBar.ClosePrice, lastBar.Time);
        }

        public async Task<IEnumerable<string>> GetProvidersAsync(CancellationToken cancellationToken)
        {
            var url = $"/api/instruments/v1/providers";

            var response = await SendAuthorizedGetAsync<PagedResponse<string>>(url, cancellationToken);

            if (response?.Data == null) return Enumerable.Empty<string>();

            return response.Data.ToList();
        }

        private async Task<T?> SendAuthorizedGetAsync<T>(string relativeUrl, CancellationToken cancellationToken)
        {
            var token = await _authService.GetAccessTokenAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var fullUrl = $"{_settings.BaseUrl}{relativeUrl}";

            return await _httpClient.GetFromJsonAsync<T>(fullUrl, cancellationToken);
        }
    }
}
