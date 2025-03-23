namespace BitcoinPriceManager.Application.BitcoinPrices.Queries.GetByTimestampRange;

public sealed class GetByTimestampRangeResponse
{
    public DateTime Timestamp { get; set; }
    public decimal Price { get; set; }
}
