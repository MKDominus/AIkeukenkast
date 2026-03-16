namespace api.Application.DTOs;

public class IngredientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsHazardous { get; set; }
    public double Concentration { get; set; }
}

public class CreateIngredientDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsHazardous { get; set; }
    public double Concentration { get; set; }
}

public class UpdateIngredientDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsHazardous { get; set; }
    public double Concentration { get; set; }
}
