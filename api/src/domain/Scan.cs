namespace api.Domain;

public class Scan
{
    public int Id { get; set; }
    public DateTime ScanDate { get; set; }
    public string ImageUrl { get; set; } 
    public int? MunicipalityId { get; set; } 
    public Municipality? Municipality { get; set; }
    public ICollection<DetectedProduct> DetectedProducts { get; set; } = new List<DetectedProduct>();
}
