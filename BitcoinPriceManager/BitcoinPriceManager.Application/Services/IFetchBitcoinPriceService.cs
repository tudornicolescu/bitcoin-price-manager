namespace BitcoinPriceManager.Application.Services;

public interface IFetchBitcoinPriceService
{
    Task<decimal> GetByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken = default);
}