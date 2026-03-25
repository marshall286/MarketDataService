using MarketDataService.Domain.Entities;

namespace MarketDataService.Application.Interfaces
{
    public interface IFintachartsRestClient
    {
        Task<Asset> GetForexAssetsAsync();
        Task<(decimal Price, DateTime LastUpdate)?> GetLastHistoricalPriceAsync(Guid instrumentId, string provider, CancellationToken cancellationToken);
    }
}
