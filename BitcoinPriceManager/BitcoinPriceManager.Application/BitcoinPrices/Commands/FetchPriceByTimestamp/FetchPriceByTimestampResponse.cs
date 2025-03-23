namespace BitcoinPriceManager.Application.BitcoinPrices.Commands.FetchPriceByTimestamp;

public sealed record FetchPriceByTimestampResponse(Guid Id, DateTime Timestamp, decimal Price);