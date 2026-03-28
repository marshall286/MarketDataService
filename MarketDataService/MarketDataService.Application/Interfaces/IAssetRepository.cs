using MarketDataService.Domain.Entities;

namespace MarketDataService.Application.Interfaces;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAllAssetsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Asset>> GetAssetsByProviderAsync(string provider, CancellationToken cancellationToken);
    Task<Asset?> GetBySymbolAndProviderAsync(string symbol, string provider, CancellationToken cancellationToken);
}
