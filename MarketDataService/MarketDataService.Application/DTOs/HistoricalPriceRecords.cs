namespace MarketDataService.Application.DTOs;

public record HistoricalPriceRequest(Guid InstrumentId, string Provider);

public record HistoricalPriceResponse(decimal Price, DateTime LastUpdate);