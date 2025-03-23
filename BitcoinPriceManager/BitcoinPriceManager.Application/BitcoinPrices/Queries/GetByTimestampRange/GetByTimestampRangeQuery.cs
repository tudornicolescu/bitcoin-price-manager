namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetByTimestampRange;

public sealed class GetByTimestampRangeQuery : IQuery<IEnumerable<GetByTimestampRangeResponse>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
