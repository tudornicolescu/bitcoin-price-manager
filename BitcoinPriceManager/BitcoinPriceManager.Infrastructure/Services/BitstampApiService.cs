using BitcoinPriceManager.Application.Services;
using BitcoinPriceManager.SharedKernel.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitcoinPriceManager.Infrastructure.Services;

public class BitstampApiService(HttpClient httpClient)
    : IExternalApiService
{
    private const string _baseEndpoint = "https://www.bitstamp.net/api/v2/ohlc/btcusd/";

    public async Task<decimal?> FetchBitcoinPriceAsync(DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var requestTimestamp = timestamp.ToUnixTimeSeconds();

        var endpoint = $"{_baseEndpoint}?step=3600&limit=1&start={requestTimestamp}";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Add("accept", "application/json");

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

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
        catch
        {
            return null;
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
