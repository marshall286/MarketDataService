using MarketDataService.Application.DTOs;
using MarketDataService.Application.Interfaces;

namespace MarketDataService.Application.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _repository;
    private readonly IFintachartsRestClient _restClient;
    private readonly ICacheProvider _cache;

    public AssetService(IAssetRepository repository, IFintachartsRestClient restClient, ICacheProvider cache)
    {
        _repository = repository;
        _restClient = restClient;
        _cache = cache;
    }

    public async Task<IEnumerable<AssetResponseDto>> GetAssetsAsync(string provider, CancellationToken cancellationToken)
    {
        var assets = await _repository.GetAssetsByProviderAsync(provider, cancellationToken);
        return assets.Select(a => new AssetResponseDto(a.Id, a.Symbol, a.Description));
    }

    public async Task<AssetPriceResponseDto?> GetAssetPriceAsync(string symbol, string provider, CancellationToken cancellationToken)
    {
        var cleanSymbol = symbol
            .Replace("/", "")
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "")
            .Trim()
            .ToUpper();

        var cleanProvider = provider.Trim().ToLower();

        var asset = await _repository.GetBySymbolAndProviderAsync(cleanSymbol, cleanProvider, cancellationToken);

        if (asset == null) return null;

        var cacheKey = $"PRICE_{asset.Id}_{cleanProvider}";

        if (_cache.TryGetValue<CachedPriceDto>(cacheKey, out var cachedPrice) && cachedPrice != null)
        {
            return new AssetPriceResponseDto(asset.Symbol, cleanProvider, cachedPrice.Price, cachedPrice.Timestamp, "WebSocket (Cache)");
        }

        var request = new HistoricalPriceRequest(asset.Id, cleanProvider);
        var priceResponse = await _restClient.GetLastHistoricalPriceAsync(request, cancellationToken);

        if (priceResponse == null) return null;

        return new AssetPriceResponseDto(asset.Symbol, cleanProvider, priceResponse.Price, priceResponse.LastUpdate, "REST API (Count-back)");
    }
}