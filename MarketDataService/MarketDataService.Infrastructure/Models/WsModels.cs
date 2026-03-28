using System.Text.Json.Serialization;

namespace MarketDataService.Infrastructure.Models;

public record FintaSubscriptionMessage(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("instrumentId")] Guid InstrumentId,
    [property: JsonPropertyName("provider")] string Provider,
    [property: JsonPropertyName("subscribe")] bool Subscribe,
    [property: JsonPropertyName("kinds")] string[] Kinds
);

public record FintaWsResponse(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("instrumentId")] Guid? InstrumentId,
    [property: JsonPropertyName("provider")] string? Provider,
    [property: JsonPropertyName("last")] FintaPriceData? Last,
    [property: JsonPropertyName("ask")] FintaPriceData? Ask,
    [property: JsonPropertyName("bid")] FintaPriceData? Bid
);

public record FintaPriceData(
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("timestamp")] DateTime Timestamp
);