namespace api.Domain;

public record ProductWarningLabel
{
    public required string Type { get; init; }
    public required string Description { get; init; }
}