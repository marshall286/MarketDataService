using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using MarketDataService.Application.DTOs;
using MarketDataService.Application.Interfaces;
using MarketDataService.Infrastructure.Configuration;
using MarketDataService.Infrastructure.Models;
using MarketDataService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MarketDataService.Infrastructure.Integrations;

public class FintachartsWebSocketService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FintachartsSettings _settings;
    private readonly ICacheProvider _cache;

    public FintachartsWebSocketService( IServiceProvider serviceProvider, IOptions<FintachartsSettings> options, ICacheProvider cache)
    {
        _serviceProvider = serviceProvider;
        _settings = options.Value;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<FintachartsAuthService>();

                var token = await authService.GetAccessTokenAsync(cancellationToken);

                using var client = new ClientWebSocket();
                var wsUri = new Uri($"{_settings.WsUrl}/api/streaming/ws/v1/realtime?token={token}");

                await client.ConnectAsync(wsUri, cancellationToken);

                await SubscribeToAssets(client, scope.ServiceProvider, cancellationToken);
                await ReceiveMessages(client, cancellationToken);
            }
            catch (Exception ex)
            {
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task SubscribeToAssets(ClientWebSocket client, IServiceProvider provider, CancellationToken cancellationToken)
    {
        var repository = provider.GetRequiredService<IAssetRepository>();
        var assets = await repository.GetAllAssetsAsync(cancellationToken);

        int idCounter = 1;
        foreach (var asset in assets)
        {
            foreach (var targetProvider in asset.Providers)
            {
                var subMessage = new FintaSubscriptionMessage(
                    Type: "l1-subscription",
                    Id: idCounter.ToString(),
                    InstrumentId: asset.Id,
                    Provider: targetProvider,
                    Subscribe: true,
                    Kinds: new[] { "last", "ask", "bid" }
                );

                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(subMessage));
                await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
                idCounter++;
            }
        }
    }

    private async Task ReceiveMessages(ClientWebSocket client, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 8];

        while (client.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            if (result.MessageType == WebSocketMessageType.Close) break;

            var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                var response = JsonSerializer.Deserialize<FintaWsResponse>(messageJson);

                if (response?.Type == "l1-update" && response.InstrumentId.HasValue && !string.IsNullOrEmpty(response.Provider))
                {
                    var priceData = response.Last ?? response.Ask ?? response.Bid;

                    if (priceData != null)
                    {
                        var cacheKey = $"PRICE_{response.InstrumentId.Value}_{response.Provider}";
                        var cachedPrice = new CachedPriceDto(priceData.Price, priceData.Timestamp);

                        _cache.Set(cacheKey, cachedPrice, TimeSpan.FromMinutes(5));
                    }
                }
            }
            catch (JsonException) {}
        }
    }
}