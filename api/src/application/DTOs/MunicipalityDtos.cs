namespace api.Application.DTOs;

public class MunicipalityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Population { get; set; }
}

public class CreateMunicipalityDto
{
    public required string Name { get; set; }
    public int Population { get; set; }
}

public class UpdateMunicipalityDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Population { get; set; }
}
