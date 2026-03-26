using MarketDataService.Application.Interfaces;
using MarketDataService.Domain.Entities;
using MarketDataService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketDataService.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IFintachartsRestClient _restClient;

    public DatabaseSeeder( ApplicationDbContext dbContext, IFintachartsRestClient restClient)
    {
        _dbContext = dbContext;
        _restClient = restClient;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.Assets.AnyAsync(cancellationToken)) return;

        var providers = await _restClient.GetProvidersAsync(cancellationToken);

        var uniqueAssets = new Dictionary<Guid, Asset>();

        foreach (var provider in providers)
        {
            var assetsFromApi = await _restClient.GetAssetsAsync(provider, cancellationToken);

            foreach (var asset in assetsFromApi)
            {
                if (!uniqueAssets.ContainsKey(asset.Id))
                {
                    uniqueAssets.Add(asset.Id, asset);
                }
            }
        }

        var finalAssetsList = uniqueAssets.Values.ToList();

        if (!finalAssetsList.Any()) return;

        await _dbContext.Assets.AddRangeAsync(finalAssetsList, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}