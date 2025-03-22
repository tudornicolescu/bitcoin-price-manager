using BitcoinPriceManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BitcoinPriceManager.Infrastructure.Data.Configurations;

/// <summary>
/// Custom configuration for BitcoinPrice entity
/// </summary>
public class BitcoinPriceConfiguration : IEntityTypeConfiguration<BitcoinPrice>
{
    public void Configure(EntityTypeBuilder<BitcoinPrice> builder)
    {
        // primary key
        builder.HasKey(bp => bp.Id);

        // indexing timestamp for fast lookups
        builder.HasIndex(bp => bp.Timestamp);
    }
}
