using MarketDataService.Application.Interfaces;
using MarketDataService.Application.Services;
using MarketDataService.Infrastructure.Configuration;
using MarketDataService.Infrastructure.Data;
using MarketDataService.Infrastructure.Integrations;
using MarketDataService.Infrastructure.Persistence;
using MarketDataService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FintachartsSettings>(
    builder.Configuration.GetSection("FintachartsApi"));
builder.Services.AddHttpClient<FintachartsAuthService>();
builder.Services.AddHttpClient<IFintachartsRestClient, FintachartsRestClient>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ICacheProvider, CacheProvider> ();
builder.Services.AddScoped<IAssetService, AssetService> ();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

    var cancellationToken = app.Lifetime.ApplicationStopping;

    await seeder.SeedAsync(cancellationToken);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
