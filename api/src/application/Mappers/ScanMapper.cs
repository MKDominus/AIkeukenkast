using api.Application.DTOs;
using api.Domain;

namespace api.Application.Mappers;

public static class ScanMapper
{
    public static ScanDto ToDto(Scan s) => new ScanDto
    {
        Id = s.Id,
        ScanDate = s.ScanDate,
        ImageUrl = s.ImageUrl,
        MunicipalityId = s.MunicipalityId,
        Municipality = s.Municipality != null ? new MunicipalityDto
        {
            Id = s.Municipality.Id,
            Name = s.Municipality.Name,
            Population = s.Municipality.Population
        } : null,
        DetectedProducts = s.DetectedProducts.Select(dp => new DetectedProductDto
        {
            Id = dp.Id,
            ProductId = dp.ProductId,
            Confidence = dp.Confidence,
            Count = dp.Count,
            Product = dp.Product != null ? new ProductDto
            {
                ProductId = dp.Product.ProductId,
                ProductName = dp.Product.ProductName,
                ProductType = dp.Product.ProductType,
                ImageURL = dp.Product.ImageURL,
                RiskLevel = dp.Product.RiskLevel.ToString(),
                WarningLabels = dp.Product.WarningLabels.Select(label => new ProductWarningLabelDto
                {
                    Type = label.Type,
                    Description = label.Description
                }).ToList(),
                Dangers = dp.Product.Dangers,
                Precautions = dp.Product.Precautions,
                Alternatives = dp.Product.Alternatives,
                Ingredients = dp.Product.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    IsHazardous = i.IsHazardous,
                    Concentration = i.Concentration
                }).ToList()
            } : null
        }).ToList()
    };
}