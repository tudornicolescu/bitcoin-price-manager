using BitcoinPriceManager.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using Xunit;

namespace BitcoinPriceManager.Tests;

public class BitfinexApiServiceTests
{
    private readonly Mock<ILogger<BitfinexApiService>> _mockLogger;

    public BitfinexApiServiceTests()
    {
        _mockLogger = new Mock<ILogger<BitfinexApiService>>();
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsThirdPriceInArray_WhenResponseIs200OK()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Create mock response for http status code 200
        var mockJsonResponse = "[[1634236800000, 45000, 46000, 44000, 45500, 1000]]";

        // Setup mocked response
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
        var service = new BitfinexApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.NotNull(price);
        Assert.Equal(46000m, price); // Check if the returned price is the third in the mocked array since we're looking for the "close" field
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ThrowsError_WhenResponseIsNot200()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Create mock response for a Bad Request
        // Any other status code than 200 should throw an error
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
        var service = new BitfinexApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.FetchBitcoinPriceAsync(timestamp));
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsNull_WhenResponseIsEmptyArray()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock an empty array response: [] when status code is 200
        var mockJsonResponse = "[]";
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
        var service = new BitfinexApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.Null(price); // Should return null if the JSON array is empty
    }

    [Fact]
    public async Task FetchBitcoinPriceAsync_ReturnsNull_WhenResponseIsNotArray()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();

        // Mock a non-array response when status code is 200
        var mockJsonResponse = "{\"error\": \"Invalid response\"}";
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
        var service = new BitfinexApiService(httpClient, _mockLogger.Object);

        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);

        // Act
        var price = await service.FetchBitcoinPriceAsync(timestamp);

        // Assert
        Assert.Null(price); // Should return null if the JSON is not a valid array
    }
}
