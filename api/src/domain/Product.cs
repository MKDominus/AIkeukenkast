namespace api.Domain; 

public class Product
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string ProductType { get; set; }
    public string? ImageURL { get; set; }
    public required ProductRiskLevel RiskLevel { get; set; }

    public List<ProductWarningLabel> WarningLabels { get; set; } = new();
    public List<string> Dangers { get; set; } = new();
    public List<string> Precautions { get; set; } = new();
    public List<string> Alternatives { get; set; } = new();

    public List<Ingredient> Ingredients { get; set; } = new();
}
