namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetByTimestampRange;

public sealed record GetByTimestampRangeResponse(DateTime Timestamp, decimal Price);