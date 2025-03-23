namespace BitcoinPriceManager.Infrastructure.Services;

public class BitstampApiService(HttpClient httpClient, ILogger<BitstampApiService> logger)
    : IExternalApiService
{
    private const string _baseEndpoint = "https://www.bitstamp.net/api/v2/ohlc/btcusd/";

    /// <summary>
    /// Method for fetching price from Bitstamp external API
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<decimal?> FetchBitcoinPriceAsync(DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var requestTimestamp = timestamp.ToUnixTimeSeconds();

        var endpoint = $"{_baseEndpoint}?step=3600&limit=1&start={requestTimestamp}";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Add("accept", "application/json");

        try
        {
            logger.LogInformation("External API call: {Uri}", endpoint);

            using var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            logger.LogInformation("Response string: {Response}", jsonString);

            var ohlcResponse = JsonSerializer.Deserialize<BitstampResponse>(jsonString);

            if (ohlcResponse is not null
                && ohlcResponse.Data is not null
                && ohlcResponse.Data.Ohlc.Count != 0)
            {
                var price = ohlcResponse.Data.Ohlc.First().Close;

                if (decimal.TryParse(price, out var result))
                {
                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while getting the API response");

            throw;
        }

        return null;
    }

    private class BitstampResponse
    {
        [JsonPropertyName("data")]
        public OhlcModel Data { get; set; }
    }

    private class OhlcModel
    {
        [JsonPropertyName("pair")]
        public string Pair { get; set; }

        [JsonPropertyName("ohlc")]
        public List<OhlcItem> Ohlc { get; set; }
    }

    private class OhlcItem
    {
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("open")]
        public string Open { get; set; }

        [JsonPropertyName("high")]
        public string High { get; set; }

        [JsonPropertyName("low")]
        public string Low { get; set; }

        [JsonPropertyName("close")]
        public string Close { get; set; }

        [JsonPropertyName("volume")]
        public string Volume { get; set; }
    }
}