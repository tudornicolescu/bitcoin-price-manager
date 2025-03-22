using BitcoinPriceManager.Infrastructure.Data;
using BitcoinPriceManager.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
