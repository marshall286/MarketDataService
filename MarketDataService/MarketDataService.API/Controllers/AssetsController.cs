using MarketDataService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace MarketDataService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAssets(string provider, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(provider))
            return BadRequest(new { Error = "Must specify provider" });

        var data = await _assetService.GetAssetsAsync(provider, cancellationToken);
        return Ok(data);
    }

    [HttpGet("price")]
    public async Task<IActionResult> GetAssetPrice(string symbol, string provider, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(symbol) || string.IsNullOrWhiteSpace(provider))
            return BadRequest(new { Error = "Must specify symbol and provider" });

        var data = await _assetService.GetAssetPriceAsync(symbol, provider, cancellationToken);

        if (data == null)
            return NotFound(new { Error = $"instrument '{symbol}' from the provider '{provider}' is not found." });

        return Ok(data);
    }
}