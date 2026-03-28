using System.Text.Json.Serialization;

namespace MarketDataService.Infrastructure.Models;

public record PagedResponse<T>(
[property: JsonPropertyName("data")] List<T> Data
);
