using MarketDataService.Infrastructure.Configuration;
using MarketDataService.Infrastructure.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace MarketDataService.Infrastructure.Services;

public class FintachartsAuthService
{
    private readonly HttpClient _httpClient;
    private readonly FintachartsSettings _settings;
    private readonly IMemoryCache _cache;

    private const string TokenCacheKey = "FintaToken";

    public FintachartsAuthService(
        HttpClient httpClient,
        IOptions<FintachartsSettings> options,
        IMemoryCache cache)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _cache = cache;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TokenCacheKey,out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
        {
            return cachedToken; 
        }

        var requestData = new Dictionary<string, string>
        {
            { "grant_type", "password" }, 
            { "client_id", "app-cli" },
            { "username", _settings.Username },
            { "password", _settings.Password } 
        };

        var authUrl = $"{_settings.BaseUrl}/identity/realms/fintatech/protocol/openid-connect/token";

        var request = new HttpRequestMessage(HttpMethod.Post, authUrl)
        { 
            Content = new FormUrlEncodedContent(requestData) 
        };

        var response = await _httpClient.SendAsync(request, cancellationToken); 

        response.EnsureSuccessStatusCode(); 

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);

        if (tokenResponse == null ||  string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            throw new InvalidOperationException("The Token is null!");
        }

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 30));

         _cache.Set(TokenCacheKey, tokenResponse.AccessToken, cacheOptions);

        return tokenResponse.AccessToken;
    }
}