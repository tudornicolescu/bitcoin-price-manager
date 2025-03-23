using BitcoinPriceManager.Application;
using BitcoinPriceManager.Infrastructure;
using BitcoinPriceManager.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register services from application and infrastructure layers
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitcoin Price API v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// apply pending migrations to the database
app.MigrateDatabase();

app.Run();

