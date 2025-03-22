using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinPriceManager.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply any pending migrations
        dbContext.Database.MigrateAsync().GetAwaiter().GetResult();
    }
}
