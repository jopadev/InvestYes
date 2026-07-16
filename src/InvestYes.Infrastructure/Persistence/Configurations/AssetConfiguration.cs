using InvestYes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace InvestYes.Infrastructure.Persistence.Configurations;


public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(x => x.AssetId);

        builder.Property(x => x.AssetId)
            .ValueGeneratedNever();

        builder.Property(x => x.Ticker)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(x => x.Ticker)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.CurrentPrice)
            .HasPrecision(18, 4);

        builder.Property(x => x.DividendYield)
            .HasPrecision(18, 4);

        builder.Property(x => x.PVP)
            .HasPrecision(18, 4);

        builder.Property(x => x.Liquidity)
            .HasPrecision(18, 2);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
