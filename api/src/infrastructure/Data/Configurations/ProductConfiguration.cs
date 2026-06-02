using api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace api.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.ProductType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.ImageURL)
            .HasMaxLength(2048);

        builder.Property(p => p.RiskLevel)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.WarningLabels)
            .HasConversion(new ValueConverter<List<ProductWarningLabel>, string>(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<ProductWarningLabel>>(value, (JsonSerializerOptions?)null) ?? new List<ProductWarningLabel>()))
            .Metadata.SetValueComparer(new ValueComparer<List<ProductWarningLabel>>(
                (left, right) => left!.SequenceEqual(right!),
                value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.Type, item.Description)),
                value => value.ToList()));

        builder.Property(p => p.Dangers)
            .HasConversion(new ValueConverter<List<string>, string>(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>()))
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (left, right) => left!.SequenceEqual(right!),
                value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
                value => value.ToList()));

        builder.Property(p => p.Precautions)
            .HasConversion(new ValueConverter<List<string>, string>(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>()))
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (left, right) => left!.SequenceEqual(right!),
                value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
                value => value.ToList()));

        builder.Property(p => p.Alternatives)
            .HasConversion(new ValueConverter<List<string>, string>(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>()))
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (left, right) => left!.SequenceEqual(right!),
                value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
                value => value.ToList()));

        builder.HasMany(p => p.Ingredients)
            .WithMany(); 
    }
}
