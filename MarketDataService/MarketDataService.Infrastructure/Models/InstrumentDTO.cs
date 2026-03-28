using System.Text.Json.Serialization;

namespace MarketDataService.Infrastructure.Models;

public record InstrumentDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("mappings")] Dictionary<string, object> Mappings
);