using api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Infrastructure.Data.Configurations;

public class DetectedProductConfiguration : IEntityTypeConfiguration<DetectedProduct>
{
    public void Configure(EntityTypeBuilder<DetectedProduct> builder)
    {
        builder.HasKey(dp => dp.Id);

        builder.HasOne(dp => dp.Product)
            .WithMany()
            .HasForeignKey(dp => dp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("DetectedProducts");
    }
}
