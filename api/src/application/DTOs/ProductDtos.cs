namespace api.Application.DTOs;

public class ProductWarningLabelDto
{
    public required string Type { get; set; }
    public required string Description { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductType { get; set; }
    public string? Supplier { get; set; }
    public string? DangerSymbol { get; set; }
    public string? ImageURL { get; set; }
    public required string RiskLevel { get; set; }
    public List<ProductWarningLabelDto> WarningLabels { get; set; } = new();
    public List<string> Dangers { get; set; } = new();
    public List<string> Precautions { get; set; } = new();
    public List<string> Alternatives { get; set; } = new();
    public List<IngredientDto> Ingredients { get; set; } = new();
}

public class CreateProductDto
{
    public required string ProductName { get; set; }
    public required string ProductType { get; set; }
    public string? Supplier { get; set; }
    public string? DangerSymbol { get; set; }
    public string? ImageURL { get; set; }
    public required string RiskLevel { get; set; }
    public List<ProductWarningLabelDto> WarningLabels { get; set; } = new();
    public List<string> Dangers { get; set; } = new();
    public List<string> Precautions { get; set; } = new();
    public List<string> Alternatives { get; set; } = new();
    public List<int>? IngredientIds { get; set; }
}

public class UpdateProductDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductType { get; set; }
    public string? Supplier { get; set; }
    public string? DangerSymbol { get; set; }
    public string? ImageURL { get; set; }
    public required string RiskLevel { get; set; }
    public List<ProductWarningLabelDto> WarningLabels { get; set; } = new();
    public List<string> Dangers { get; set; } = new();
    public List<string> Precautions { get; set; } = new();
    public List<string> Alternatives { get; set; } = new();
    public List<int>? IngredientIds { get; set; }
}

public class ProductCategoryCountDto
{
    public string ProductType { get; set; } = string.Empty;
    public int Count { get; set; }
}
