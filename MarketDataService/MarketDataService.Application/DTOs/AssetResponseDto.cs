namespace MarketDataService.Application.DTOs;

public record AssetResponseDto(Guid Id, string Symbol, string? Description);

public record AssetPriceResponseDto(string Symbol, string Provider, decimal Price, DateTime LastUpdate, string Source);
