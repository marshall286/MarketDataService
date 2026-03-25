using System.Text.Json.Serialization;

namespace MarketDataService.Infrastructure.Models;

public record CountBackResponse(
    [property: JsonPropertyName("data")] List<HistoricalBarDto> Data);

public record HistoricalBarDto(
    [property: JsonPropertyName("t")] DateTime Time,
    [property: JsonPropertyName("c")] decimal ClosePrice);
