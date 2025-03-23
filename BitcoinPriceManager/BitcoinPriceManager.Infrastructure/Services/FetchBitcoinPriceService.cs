namespace BitcoinPriceManager.Infrastructure.Services;

public class FetchBitcoinPriceService(IServiceProvider serviceProvider)
    : IFetchBitcoinPriceService
{
    public async Task<decimal> GetByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var prices = new List<decimal>();

        var externalApiServices = serviceProvider.GetServices<IExternalApiService>();
        foreach (var externalApiService in externalApiServices)
        {
            var price = await externalApiService.FetchBitcoinPriceAsync(timestamp, cancellationToken);
            if (price is not null)
            {
                prices.Add(price.Value);
            }
        }

        return prices.AggregatePrices();
    }
}