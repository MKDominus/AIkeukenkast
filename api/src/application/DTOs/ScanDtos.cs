namespace api.Application.DTOs;

public class ScanDto
{
    public int Id { get; set; }
    public DateTime ScanDate { get; set; }
    public required string ImageUrl { get; set; }
    public int? MunicipalityId { get; set; }
    public MunicipalityDto? Municipality { get; set; } 
    public int? UserId { get; set; }
    public UserDto? User { get; set; } 
    public ICollection<DetectedProductDto> DetectedProducts { get; set; } = new List<DetectedProductDto>();
}

public class CreateScanDto
{
    public required string ImageUrl { get; set; }
    public int? MunicipalityId { get; set; }
    public int? UserId { get; set; }
    public List<CreateDetectedProductDto> DetectedProducts { get; set; } = new();
}

public class UpdateScanDto
{
    public int Id { get; set; }
    public required string ImageUrl { get; set; }
    public int? MunicipalityId { get; set; }
    public int? UserId { get; set; }
}
