using BitcoinPriceManager.Application.Services;
using BitcoinPriceManager.SharedKernel.Extensions;
using System.Text.Json;

namespace BitcoinPriceManager.Infrastructure.Services;

public class BitfinexApiService(HttpClient httpClient)
    : IExternalApiService
{
    private const string _baseEndpoint = "https://api.bitfinex.com/v2/candles/trade:1h:tBTCUSD/hist";

    public async Task<decimal?> FetchBitcoinPriceAsync(DateTime timestamp, CancellationToken cancellationToken = default)
    {
        var startTimestamp = timestamp.ToUnixTimeMilliseconds();
        var endTimestamp = timestamp.AddHours(1).ToUnixTimeMilliseconds();

        var endpoint = $"{_baseEndpoint}?start={startTimestamp}&end={endTimestamp}&limit=1";

        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Add("accept", "application/json");

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync(cancellationToken);

            // Parse JSON response
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
        catch
        {
            return null;
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
