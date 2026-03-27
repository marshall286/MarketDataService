using MarketDataService.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace MarketDataService.Infrastructure.Persistence;

public class CacheProvider : ICacheProvider
{
    private readonly IMemoryCache _cache;

    public CacheProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set<T>(string key, T value, TimeSpan? absoluteExpiration)
    {
        if (absoluteExpiration.HasValue)
        {
            _cache.Set(key, value, absoluteExpiration.Value);
        }
        else
        {
            _cache.Set(key, value);
        }
    }
}