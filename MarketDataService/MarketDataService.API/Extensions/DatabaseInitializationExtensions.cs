using MarketDataService.Infrastructure.Data;
using MarketDataService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketDataService.API.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(CancellationToken.None);
    }
}