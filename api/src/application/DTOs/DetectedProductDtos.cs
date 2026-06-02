namespace api.Application.DTOs;

public class DetectedProductDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public double Confidence { get; set; }
    public int Count { get; set; }
    
    //maybe not needed.. for detail rn leaving it in. 
    public ProductDto? Product { get; set; }
}

public class CreateDetectedProductDto
{
    public int ScanId { get; set; } 
    public int ProductId { get; set; }
    public double Confidence { get; set; }
    public int Count { get; set; }
}

public class UpdateDetectedProductDto
{
    public int Id { get; set; }
    public double Confidence { get; set; }
    public int Count { get; set; }
}
