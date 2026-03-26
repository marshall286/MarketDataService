using MarketDataService.Application.DTOs;
using MarketDataService.Domain.Entities;

namespace MarketDataService.Application.Interfaces
{
    public interface IFintachartsRestClient
    {
        Task<IEnumerable<Asset>> GetForexAssetsAsync(string provider, CancellationToken cancellationToken);
        Task<HistoricalPriceResponse?> GetLastHistoricalPriceAsync(HistoricalPriceRequest request, CancellationToken cancellationToken);
    }
}
