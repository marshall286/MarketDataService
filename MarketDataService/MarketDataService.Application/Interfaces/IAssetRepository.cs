using MarketDataService.Domain.Entities;

namespace MarketDataService.Application.Interfaces;

public interface IAssetRepository
{
    Task<IEnumerable<Asset>> GetAssetsByProviderAsync(string provider, CancellationToken ct = default);
    Task<Asset?> GetBySymbolAndProviderAsync(string symbol, string provider, CancellationToken ct = default);
}
