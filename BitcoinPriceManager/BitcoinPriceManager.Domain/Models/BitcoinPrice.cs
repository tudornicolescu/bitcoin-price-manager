using BitcoinPriceManager.Domain.Abstractions;
using BitcoinPriceManager.SharedKernel.Extensions;

namespace BitcoinPriceManager.Domain.Models;

public class BitcoinPrice : Entity<Guid>
{
    /// <summary>
    /// When the price was recorded
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Bitcoin price at the recorded timestamp
    /// </summary>
    public decimal Price { get; private set; }

    // Required for EF Core
    private BitcoinPrice() { }

    public BitcoinPrice(DateTime timestamp, decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greated than zero.");

        Id = Guid.NewGuid();
        Timestamp = timestamp.NormalizeToHour();
        Price = price;
    }
}
