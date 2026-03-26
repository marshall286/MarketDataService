namespace MarketDataService.Application.DTOs;

public record HistoricalPriceRequest(Guid InstrumentId, string Provider);