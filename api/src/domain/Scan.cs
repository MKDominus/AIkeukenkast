namespace api.Domain;

public class Scan
{
    public int Id { get; set; }
    public DateTime ScanDate { get; set; }
    public required string ImageUrl { get; set; } 
    public int? MunicipalityId { get; set; } 
    public Municipality? Municipality { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<DetectedProduct> DetectedProducts { get; set; } = new List<DetectedProduct>();
}
