namespace api.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public int SustainabilityScore { get; set; }
    public bool IsSustainable { get; set; }
    public string? SafetyWarnings { get; set; }
    public List<IngredientDto> Ingredients { get; set; } = new();
}

public class CreateProductDto
{
    public required string Name { get; set; }
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = "General";
    public int SustainabilityScore { get; set; }
    public bool IsSustainable { get; set; }
    public string? SafetyWarnings { get; set; }
    public List<int>? IngredientIds { get; set; }
}

public class UpdateProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }
    public required string Category { get; set; }
    public int SustainabilityScore { get; set; }
    public bool IsSustainable { get; set; }
    public string? SafetyWarnings { get; set; }
    public List<int>? IngredientIds { get; set; }
}
