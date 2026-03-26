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
        public async Task<IEnumerable<Asset>> GetForexAssetsAsync(string provider, CancellationToken cancellationToken)
        {
            var token = await _authService.GetAccessTokenAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_settings.BaseUrl}/api/instruments/v1/instruments?provider={provider}&kind=forex";

            var response = await _httpClient.GetFromJsonAsync<PagedResponse<InstrumentDto>>(url, cancellationToken);

            if (response?.Data == null) return null;

            return response.Data.Select(dto => new Asset
            {
                Id = dto.Id,
                Symbol = dto.Symbol,
                Description = dto.Description
            }).ToList();
        }

        public async Task<HistoricalPriceResponse?> GetLastHistoricalPriceAsync(HistoricalPriceRequest request, CancellationToken cancellationToken)
        {
            var token = await _authService.GetAccessTokenAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"{_settings.BaseUrl}/api/bars/v1/bars/count-back?instrumentId={request.InstrumentId}&provider={request.Provider}&interval=1&periodicity=minute&barsCount=1";

            var response = await _httpClient.GetFromJsonAsync<PagedResponse<HistoricalBarDto>>(url, cancellationToken);

            var lastBar = response?.Data?.FirstOrDefault();

            if (lastBar == null) return null;

            return new HistoricalPriceResponse(lastBar.ClosePrice, lastBar.Time);
        }
    }
}
