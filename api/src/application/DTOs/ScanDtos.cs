using Microsoft.AspNetCore.Http;

namespace api.Application.DTOs;

public class ScanDto
{
    public int Id { get; set; }
    public DateTime ScanDate { get; set; }
    public required string ImageUrl { get; set; }
    public int? MunicipalityId { get; set; }
    public MunicipalityDto? Municipality { get; set; } 
    public ICollection<DetectedProductDto> DetectedProducts { get; set; } = new List<DetectedProductDto>();
}

public class CreateScanDto
{
    public required string ImageUrl { get; set; }
    public int? MunicipalityId { get; set; }
    public List<CreateDetectedProductDto> DetectedProducts { get; set; } = new();
}

public class UpdateScanDto
{
    public int Id { get; set; }
    public required string ImageUrl { get; set; }
    public int? MunicipalityId { get; set; }
}

public class ScanStatsDto
{
    public int TotalScans { get; set; }
    public int ProductsScanned { get; set; }
    public double AverageSafety { get; set; }
    public double AverageRisk { get; set; }
}

public class CreateScanFormDto
{
    public List<IFormFile> Images { get; set; } = new();
}
