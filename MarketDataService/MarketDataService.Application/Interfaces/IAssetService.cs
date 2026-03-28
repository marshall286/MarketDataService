using MarketDataService.Application.DTOs;

namespace MarketDataService.Application.Interfaces;

public interface IAssetService
{
    Task<IEnumerable<AssetResponseDto>> GetAssetsAsync(string provider, CancellationToken cancellationToken);
    Task<AssetPriceResponseDto?> GetAssetPriceAsync(string symbol, string provider, CancellationToken cancellationToken);
}