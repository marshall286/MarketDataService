namespace MarketDataService.Application.DTOs;

public record HistoricalPriceResponse(decimal Price, DateTime LastUpdate);
