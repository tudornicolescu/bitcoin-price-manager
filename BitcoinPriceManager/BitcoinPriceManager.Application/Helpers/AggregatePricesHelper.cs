namespace BitcoinPriceManager.Application.Helpers;

public static class AggregatePricesHelper
{
    /// <summary>
    /// Helper function to aggregate prices
    /// </summary>
    /// <param name="prices"></param>
    /// <returns></returns>
    public static decimal AggregatePrices(this List<decimal> prices)
    {
        if (prices.Count == 0)
        {
            return 0;
        }

        return prices.Average();
    }
}