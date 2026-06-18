using api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Infrastructure.Data.Configurations;

public class ScanConfiguration : IEntityTypeConfiguration<Scan>
{
    public void Configure(EntityTypeBuilder<Scan> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(20);

        builder.Property(s => s.PostalCodePermission)
            .HasDefaultValue(false);

        builder.HasOne(s => s.Municipality)
            .WithMany()
            .HasForeignKey(s => s.MunicipalityId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.HasMany(s => s.DetectedProducts)
            .WithOne(dp => dp.Scan)
            .HasForeignKey(dp => dp.ScanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
