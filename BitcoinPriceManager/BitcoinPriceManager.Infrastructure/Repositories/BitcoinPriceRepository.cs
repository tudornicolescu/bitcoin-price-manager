using BitcoinPriceManager.Application.Data;
using BitcoinPriceManager.Domain.Models;
using BitcoinPriceManager.Infrastructure.Data;
using BitcoinPriceManager.SharedKernel.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BitcoinPriceManager.Infrastructure.Repositories;

/// <summary>
/// Repository for BitcoinPrice entity database operations
/// </summary>
/// <param name="dbContext"></param>
public class BitcoinPriceRepository(ApplicationDbContext dbContext) : IBitcoinPriceRepository
{
    /// <summary>
    /// Adds a new BitcoinPrice entity to the database and calls SaveChangesAsync()
    /// </summary>
    /// <param name="bitcoinPrice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddAsync(BitcoinPrice bitcoinPrice, CancellationToken cancellationToken)
    {
        await dbContext.BitcoinPrices.AddAsync(bitcoinPrice, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the first BitcoinPrice entity with the given timestamp. If no entity is found, returns null.
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BitcoinPrice> GetByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken)
    {
        return await dbContext.BitcoinPrices
            .FirstOrDefaultAsync(e => e.Timestamp == timestamp.NormalizeToHour(), cancellationToken);
    }

    /// <summary>
    /// Retrieves a List of BitcoinPrices from the database that were recorded between a start and end interval.
    /// If none are found, returns an empty enumerable.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<BitcoinPrice>> GetByTimestampRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var startFrom = startDate.NormalizeToHour();
        var endTo = endDate.NormalizeToHour();

        return await dbContext.BitcoinPrices
            .Where(e => e.Timestamp >= startFrom && e.Timestamp <= endTo)
            .ToListAsync(cancellationToken);
    }
}
