using BitcoinPriceManager.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using Xunit;

namespace BitcoinPriceManager.Tests;

public class BitstampApiServiceTests
{
    private readonly Mock<ILogger<BitstampApiService>> _mockLogger;

    public BitstampApiServiceTests()
    {
        _mockLogger = new Mock<ILogger<BitstampApiService>>();
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsPrice_WhenResponseIs200OK()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock a valid JSON response with price data
        var mockJsonResponse = @"
        {
            ""data"": {
                ""pair"": ""btcusd"",
                ""ohlc"": [
                    {
                        ""timestamp"": ""1634236800"",
                        ""open"": ""45000"",
                        ""high"": ""46000"",
                        ""low"": ""44000"",
                        ""close"": ""45500"",
                        ""volume"": ""1000""
                    }
                ]
            }
        }";

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var service = new BitstampApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.NotNull(price);
        Assert.Equal(45500m, price); // Expected price from the JSON response
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ThrowsException_WhenResponseIsNot200()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock a non-200 response (e.g., 400 Bad Request)
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad Request")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var service = new BitstampApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.FetchBitcoinPriceAsync(timestamp));
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsNull_WhenResponseHasEmptyOhlc()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock an empty OHLC list in the response
        var mockJsonResponse = @"
        {
            ""data"": {
                ""pair"": ""btcusd"",
                ""ohlc"": []
            }
        }";

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var service = new BitstampApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.Null(price); // Should return null if the "ohlc" list is empty
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsNull_WhenResponseIsInvalidJson()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock an invalid JSON response (missing "ohlc")
        var mockJsonResponse = @"
        {
            ""data"": {
                ""pair"": ""btcusd""
            }
        }";

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var service = new BitstampApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.Null(price); // Should return null if the JSON does not contain "ohlc"
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsNull_WhenClosePriceIsInvalid()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock a response where "close" field is not a valid decimal
        var mockJsonResponse = @"
        {
            ""data"": {
                ""pair"": ""btcusd"",
                ""ohlc"": [
                    {
                        ""timestamp"": ""1634236800"",
                        ""open"": ""45000"",
                        ""high"": ""46000"",
                        ""low"": ""44000"",
                        ""close"": ""INVALID_PRICE"",
                        ""volume"": ""1000""
                    }
                ]
            }
        }";

        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(mockJsonResponse, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        var service = new BitstampApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.Null(price); // Should return null if "close" price is not a valid decimal
    }
}
