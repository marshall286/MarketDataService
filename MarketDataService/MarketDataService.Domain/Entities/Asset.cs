namespace MarketDataService.Domain.Entities;

public class Asset
{
    public Guid Id { get; set; }
    public string Symbol { get; set; }
    public string? Description { get; set; }
    public List<string> Providers { get; set; }
}
