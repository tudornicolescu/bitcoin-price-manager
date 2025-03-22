using BitcoinPriceManager.Infrastructure;
using BitcoinPriceManager.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

// register services from domain, application and infrastructure layers
builder.Services
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// apply pending migrations to the database
app.MigrateDatabase();

app.Run();

