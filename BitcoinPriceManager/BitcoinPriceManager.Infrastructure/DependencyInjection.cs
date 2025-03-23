using BitcoinPriceManager.Infrastructure.Data.Interceptors;
using BitcoinPriceManager.Infrastructure.Repositories;
using BitcoinPriceManager.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace BitcoinPriceManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var auditableEntityInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();

            options.UseSqlite(connectionString)
                .AddInterceptors(auditableEntityInterceptor);
        });

        services.AddScoped<IBitcoinPriceRepository, BitcoinPriceRepository>();

        services.AddHttpClient<BitstampApiService>();
        services.AddHttpClient<BitfinexApiService>();

        services.AddScoped<IExternalApiService, BitstampApiService>();
        services.AddScoped<IExternalApiService, BitfinexApiService>();

        services.AddScoped<IFetchBitcoinPriceService, FetchBitcoinPriceService>();

        return services;
    }
}