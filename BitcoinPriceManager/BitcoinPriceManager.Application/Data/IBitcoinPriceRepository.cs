using BitcoinPriceManager.Domain.Models;

namespace BitcoinPriceManager.Application.Data;

/// <summary>
/// Contract for repository to be used in application use-cases.
/// Concrete implementation found in Infrastructure project.
/// </summary>
public interface IBitcoinPriceRepository
{
    Task<BitcoinPrice> GetByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken);
    Task<IEnumerable<BitcoinPrice>> GetByTimestampRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    Task AddAsync(BitcoinPrice bitcoinPrice, CancellationToken cancellationToken);
}
