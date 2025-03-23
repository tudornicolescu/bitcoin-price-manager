var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register services from application and infrastructure layers
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitcoin Price API v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

// apply pending migrations to the database
app.MigrateDatabase();

// Map endpoints
app.MapGet("/price/{timestamp:datetime}", async (DateTime timestamp, IMediator mediator, CancellationToken cancellationToken) =>
{
    var query = new GetPriceByTimestampQuery(timestamp);

    var getPriceByTimestampResponse = await mediator.Send(query, cancellationToken);

    if (getPriceByTimestampResponse.Price != 0)
    {
        return Results.Ok(getPriceByTimestampResponse.Price);
    }
    else
    {
        return Results.NotFound($"No price found for timestamp {timestamp}");
    }
});

app.MapGet("/prices", async (DateTime startDate, DateTime endDate, IMediator mediator, CancellationToken cancellationToken) =>
{
    var query = new GetByTimestampRangeQuery(startDate, endDate);

    var prices = await mediator.Send(query, cancellationToken);

    if (prices.Any())
    {
        return Results.Ok(prices);
    }
    else
    {
        return Results.NotFound($"No prices found between {startDate} and {endDate}");
    }
});

app.Run();

