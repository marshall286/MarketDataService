namespace MarketDataService.Application.Interfaces;

public interface ICacheProvider
{
    bool TryGetValue<T>(string key, out T? value);

    void Set<T>(string key, T value, TimeSpan? absoluteExpiration);
}