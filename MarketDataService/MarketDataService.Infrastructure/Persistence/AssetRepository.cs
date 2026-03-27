using MarketDataService.Application.Interfaces;
using MarketDataService.Domain.Entities;
using MarketDataService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MarketDataService.Infrastructure.Persistence;

public class AssetRepository : IAssetRepository
{
    private readonly ApplicationDbContext _context;

    public AssetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Asset>> GetAssetsByProviderAsync(string provider, CancellationToken cancellationToken)
    {
        return await _context.Assets
            .AsNoTracking()
            .Where(a => a.Providers.Contains(provider))
            .ToListAsync(cancellationToken);
    }

    public async Task<Asset?> GetBySymbolAndProviderAsync(string symbol, string provider, CancellationToken ct = default)
    {
        return await _context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Symbol == symbol && a.Providers.Contains(provider), ct);
    }
}
