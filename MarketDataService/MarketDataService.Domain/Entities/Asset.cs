namespace MarketDataService.Domain.Entities;

public class Asset
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? Exchange { get; set; }
}
