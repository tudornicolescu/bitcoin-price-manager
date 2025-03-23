namespace BitcoinPriceManager.Infrastructure.Services;

public class BitfinexApiService(HttpClient httpClient, ILogger<BitfinexApiService> logger)
    : IExternalApiService
{
    private const string _baseEndpoint = "https://api.bitfinex.com/v2/candles/trade:1h:tBTCUSD/hist";

    /// <summary>
    /// Method for fetching prices for given timestamp from Bitfinex external API
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<decimal?> FetchBitcoinPriceAsync(DateTime timestamp, CancellationToken cancellationToken = default)
    {
        // create timestamps from datetime to unix time for request
        var startTimestamp = timestamp.ToUnixTimeMilliseconds();
        var endTimestamp = timestamp.AddHours(1).ToUnixTimeMilliseconds();

        var endpoint = $"{_baseEndpoint}?start={startTimestamp}&end={endTimestamp}&limit=1";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Add("accept", "application/json");

        try
        {
            logger.LogInformation("External API call: {Uri}", endpoint);

            // http call + ensure success status code
            using var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogInformation("Response string: {Response}", jsonString);

            // parse JSON response
            var jsonDocument = JsonDocument.Parse(jsonString);
            var root = jsonDocument.RootElement;

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var candle = root[0];

                var price = candle[(int)BitfinexResponseFields.Close].GetDecimal();

                // return the "Close" field
                return price;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while getting the API response");

            throw;
        }

        return null;
    }

    private enum BitfinexResponseFields
    {
        Mts = 0,
        Open,
        Close,
        High,
        Low,
        Volume
    }
}