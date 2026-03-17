namespace api.Domain; 

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Brand { get; set; }
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = "General";

    public int SustainabilityScore { get; set; }
    public bool IsSustainable { get; set; }
    public string? SafetyWarnings { get; set; }

    public List<Ingredient> Ingredients { get; set; } = new();
}
