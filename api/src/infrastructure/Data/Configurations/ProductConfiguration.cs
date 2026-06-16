using api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        builder.Property(p => p.Supplier)
            .HasMaxLength(200);

        builder.Property(p => p.ImageURL)
            .HasMaxLength(2048);

        builder.Property(p => p.RiskLevel)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        var stringListConverter = new ValueConverter<List<string>, string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null)
                     ?? new List<string>());

        var stringListComparer = new ValueComparer<List<string>>(
            (left, right) => left!.SequenceEqual(right!),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
            value => value.ToList());

        var intListConverter = new ValueConverter<List<int>, string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => JsonSerializer.Deserialize<List<int>>(value, (JsonSerializerOptions?)null)
                    ?? new List<int>());

        var intListComparer = new ValueComparer<List<int>>(
            (left, right) => left!.SequenceEqual(right!),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
            value => value.ToList());

        builder.Property(p => p.DangerSymbols)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(p => p.Dangers)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(p => p.Precautions)
            .HasConversion(stringListConverter)
            .Metadata.SetValueComparer(stringListComparer);

        builder.Property(p => p.Alternatives)
            .HasConversion(intListConverter)
            .Metadata.SetValueComparer(intListComparer);

        builder.Property(p => p.WarningLabels)
            .HasConversion(new ValueConverter<List<ProductWarningLabel>, string>(
                value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                value => JsonSerializer.Deserialize<List<ProductWarningLabel>>(value, (JsonSerializerOptions?)null)
                         ?? new List<ProductWarningLabel>()))
            .Metadata.SetValueComparer(new ValueComparer<List<ProductWarningLabel>>(
                (left, right) => left!.SequenceEqual(right!),
                value => value.Aggregate(
                    0,
                    (hash, item) => HashCode.Combine(hash, item.Type, item.Description)),
                value => value.ToList()));

        builder.HasMany(p => p.Ingredients)
            .WithMany();
    }
}