namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetPriceByTimestamp;

public sealed class GetPriceByTimestampQuery : IQuery<GetPriceByTimestampResponse>
{
    public DateTime Timestamp { get; set; }
}
