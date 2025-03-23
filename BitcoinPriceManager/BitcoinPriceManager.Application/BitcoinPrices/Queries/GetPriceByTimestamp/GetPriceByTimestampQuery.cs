namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetPriceByTimestamp;

public sealed record GetPriceByTimestampQuery(DateTime Timestamp) : IQuery<GetPriceByTimestampResponse>;