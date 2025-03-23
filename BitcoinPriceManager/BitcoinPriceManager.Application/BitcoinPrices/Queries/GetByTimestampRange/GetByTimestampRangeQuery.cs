namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetByTimestampRange;

public sealed record GetByTimestampRangeQuery(DateTime StartDate, DateTime EndDate) : IQuery<IEnumerable<GetByTimestampRangeResponse>>;