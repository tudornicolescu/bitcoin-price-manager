using BitcoinPriceManager.Application.Services;
using BitcoinPriceManager.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BitcoinPriceManager.Tests;

public class FetchBitcoinPriceServiceTests
{
    private readonly Mock<IExternalApiService> _mockBitfinexService;
    private readonly Mock<IExternalApiService> _mockBitstampService;
    private readonly ServiceProvider _serviceProvider;

    public FetchBitcoinPriceServiceTests()
    {
        _mockBitfinexService = new Mock<IExternalApiService>();
        _mockBitstampService = new Mock<IExternalApiService>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(_mockBitfinexService.Object);
        serviceCollection.AddSingleton(_mockBitstampService.Object);

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task GetByTimestampAsync_ReturnsAggregatedPrice_WhenBothApisReturnValidPrices()
    {
        // Arrange
        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);
        _mockBitfinexService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ReturnsAsync(45000m);

        _mockBitstampService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ReturnsAsync(45500m);

        var service = new FetchBitcoinPriceService(_serviceProvider);

        // Act
        var result = await service.GetByTimestampAsync(timestamp);

        // Assert
        Assert.Equal((45000m + 45500m) / 2, result); // Assuming AggregatePrices() calculates the average
    }

    [Fact]
    public async Task GetByTimestampAsync_ReturnsSinglePrice_WhenOneApiReturnsNull()
    {
        // Arrange
        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);
        _mockBitfinexService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ReturnsAsync(46000m);

        _mockBitstampService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal?)null);

        var service = new FetchBitcoinPriceService(_serviceProvider);

        // Act
        var result = await service.GetByTimestampAsync(timestamp);

        // Assert
        Assert.Equal(46000m, result); // Should return the valid price
    }

    [Fact]
    public async Task GetByTimestampAsync_ReturnsZero_WhenAllApisReturnNull()
    {
        // Arrange
        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);
        _mockBitfinexService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal?)null);

        _mockBitstampService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ReturnsAsync((decimal?)null);

        var service = new FetchBitcoinPriceService(_serviceProvider);

        // Act
        var result = await service.GetByTimestampAsync(timestamp);

        // Assert
        Assert.Equal(0m, result); // Assuming AggregatePrices() returns 0 for empty lists
    }

    [Fact]
    public async Task GetByTimestampAsync_ThrowsException_WhenAllApisFail()
    {
        // Arrange
        var timestamp = new DateTime(2023, 10, 14, 12, 0, 0);
        _mockBitfinexService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Bitfinex API error"));

        _mockBitstampService
            .Setup(x => x.FetchBitcoinPriceAsync(timestamp, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Bitstamp API error"));

        var service = new FetchBitcoinPriceService(_serviceProvider);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetByTimestampAsync(timestamp));
    }
}
