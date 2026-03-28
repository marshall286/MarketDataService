using System.Text.Json.Serialization;

namespace MarketDataService.Infrastructure.Models;

public record TokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn
);