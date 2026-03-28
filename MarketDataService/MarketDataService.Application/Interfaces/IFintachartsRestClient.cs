using MarketDataService.Application.DTOs;
using MarketDataService.Domain.Entities;

namespace MarketDataService.Application.Interfaces
{
    public interface IFintachartsRestClient
    {
        Task<IEnumerable<Asset>> GetAssetsAsync(string provider, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetProvidersAsync(CancellationToken cancellationToken);
        Task<HistoricalPriceResponse?> GetLastHistoricalPriceAsync(HistoricalPriceRequest request, CancellationToken cancellationToken);
    }
}
