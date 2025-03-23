namespace BitcoinPriceManager.Application.Services;

public interface IExternalApiService
{
    Task<decimal?> FetchBitcoinPriceAsync(DateTime timestamp, CancellationToken cancellationToken = default);
}