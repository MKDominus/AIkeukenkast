namespace api.Domain;

public class DetectedProduct
{
    public int Id { get; set; }
    public int ScanId { get; set; }
    public Scan? Scan { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public double Confidence { get; set; } 
    public int Count { get; set; } = 1;
}
