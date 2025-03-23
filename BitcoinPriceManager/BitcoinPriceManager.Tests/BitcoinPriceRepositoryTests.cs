using BitcoinPriceManager.Domain.Models;
using BitcoinPriceManager.Infrastructure.Data;
using BitcoinPriceManager.Infrastructure.Repositories;
using BitcoinPriceManager.SharedKernel.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinPriceManager.Tests;


public class BitcoinPriceRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BitcoinPriceRepository _repository;
    private readonly Random _random;

    public BitcoinPriceRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensures fresh DB per test
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _repository = new BitcoinPriceRepository(_dbContext);
        _random = new Random();
    }

    private DateTime GetRandomDateTime()
    {
        return new DateTime(
            _random.Next(2000, 2030),
            _random.Next(1, 12),
            _random.Next(1, 28),
            _random.Next(0, 23),
            0, 0, DateTimeKind.Utc
        ).NormalizeToHour();
    }

    [Fact]
    public async Task AddAsync_AddsBitcoinPriceToDatabase()
    {
        // Arrange
        var bitcoinPrice = new BitcoinPrice(GetRandomDateTime(), _random.Next(30000, 60000));

        // Act
        await _repository.AddAsync(bitcoinPrice, CancellationToken.None);
        var result = await _dbContext.BitcoinPrices.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bitcoinPrice.Timestamp, result.Timestamp);
        Assert.Equal(bitcoinPrice.Price, result.Price);
    }

    [Fact]
    public async Task GetByTimestampAsync_ReturnsBitcoinPrice_WhenExists()
    {
        // Arrange
        var timestamp = GetRandomDateTime();
        var bitcoinPrice = new BitcoinPrice(timestamp, _random.Next(30000, 60000));

        await _dbContext.BitcoinPrices.AddAsync(bitcoinPrice);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTimestampAsync(timestamp, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bitcoinPrice.Price, result.Price);
    }

    [Fact]
    public async Task GetByTimestampAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByTimestampAsync(GetRandomDateTime(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTimestampRangeAsync_ReturnsPricesWithinRange()
    {
        // Arrange
        var startDate = GetRandomDateTime();
        var endDate = startDate.AddHours(2);

        var prices = new List<BitcoinPrice>
        {
            new BitcoinPrice(startDate, _random.Next(30000, 60000)),
            new BitcoinPrice(endDate, _random.Next(30000, 60000))
        };

        await _dbContext.BitcoinPrices.AddRangeAsync(prices);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTimestampRangeAsync(startDate, endDate, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTimestampRangeAsync_ReturnsEmptyList_WhenNoMatches()
    {
        // Arrange
        var startDate = GetRandomDateTime();
        var endDate = startDate.AddHours(2);

        // Act
        var result = await _repository.GetByTimestampRangeAsync(startDate, endDate, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}