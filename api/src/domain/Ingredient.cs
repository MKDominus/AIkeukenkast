namespace api.Domain;

public class Ingredient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsHazardous { get; set; } 
    public double Concentration { get; set; } 
}
