using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScansController : ControllerBase
{
    private readonly IScanService _service;
    private readonly IProductService _productService;

    public ScansController(IScanService service, IProductService productService)
    {
        _service = service;
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScanDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScanDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ToDto(item));
    }

    //we take in image, we make image form ready, send to python AI endpoint, then receive it and make scan object.
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<ScanDto>>> Create([FromForm] CreateScanFormDto dto)
    {
        var images = dto?.Images?.Where(image => image.Length > 0).ToList() ?? new List<IFormFile>();
        if (images.Count == 0)
        {
            return BadRequest("At least one image file is required.");
        }

        using var httpClient = new HttpClient();
        using var form = new MultipartFormDataContent();
        
        // Keep streams alive for the entire outgoing request
        var streams = new List<Stream>();
        var fileContents = new List<StreamContent>();

        foreach (var image in images)
        {
            var imageStream = image.OpenReadStream();
            streams.Add(imageStream);

            var fileContent = new StreamContent(imageStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                string.IsNullOrWhiteSpace(image.ContentType)
                    ? "application/octet-stream"
                    : image.ContentType);

            fileContents.Add(fileContent);
            form.Add(fileContent, "images", image.FileName);
        }

        //hardcoded url for now, should be in config later. 
        using var response = await httpClient.PostAsync("http://127.0.0.1:8000/predict", form);
        foreach (var fileContent in fileContents) fileContent.Dispose();
        foreach (var stream in streams) stream.Dispose();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, "Prediction service returned an error.");
        }

        var payload = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(payload);
        var results = document.RootElement.GetProperty("results").EnumerateArray().ToList();
        if (results.Count == 0)
        {
            return StatusCode(StatusCodes.Status502BadGateway, "Prediction service returned no results.");
        }
        
        //we check for existing product names. All seems a bit much for keeping in controller, maybe move later
        var knownProducts = (await _productService.GetAllAsync())
            .ToDictionary(product => product.ProductName, StringComparer.OrdinalIgnoreCase);
        var createdScans = new List<ScanDto>();

        //we loop through results and make a scan for each result/picture. Im not sure if we want it this way. Might get changed.
        foreach (var result in results)
        {
            var detectedProducts = result
                .GetProperty("predictions")
                .EnumerateArray()
                .Select(prediction => new
                {
                    ProductName = prediction.GetProperty("product name").GetString(),
                    Probability = prediction.GetProperty("probability").GetDouble()
                })
                .Where(prediction => prediction.ProductName != null && knownProducts.ContainsKey(prediction.ProductName))
                .Select(prediction => new DetectedProduct
                {
                    ProductId = knownProducts[prediction.ProductName!].ProductId,
                    Confidence = prediction.Probability,
                    Count = 1
                })
                .ToList();

            var entity = new Scan
            {
                ScanDate = DateTime.UtcNow,
                ImageUrl = result.GetProperty("file name").GetString() ?? string.Empty,
                MunicipalityId = 1,
                DetectedProducts = detectedProducts
            };

            await _service.AddAsync(entity);
            createdScans.Add(ToDto(entity));
        }

        return Ok(createdScans);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateScanDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        
        entity.ImageUrl = dto.ImageUrl;
        entity.MunicipalityId = dto.MunicipalityId;
        
        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ScanStatsDto>> GetStats()
    {
        var stats = await _service.GetStatsAsync();
        return Ok(stats);
    }

    private static ScanDto ToDto(Scan s) => new ScanDto
    {
        Id = s.Id,
        ScanDate = s.ScanDate,
        ImageUrl = s.ImageUrl,
        MunicipalityId = s.MunicipalityId,
        Municipality = s.Municipality != null ? new MunicipalityDto
        {
            Id = s.Municipality.Id,
            Name = s.Municipality.Name,
            Population = s.Municipality.Population
        } : null,
        DetectedProducts = s.DetectedProducts.Select(dp => new DetectedProductDto
        {
            Id = dp.Id,
            ProductId = dp.ProductId,
            Confidence = dp.Confidence,
            Count = dp.Count,
            Product = dp.Product != null ? new ProductDto
            {
                ProductId = dp.Product.ProductId,
                ProductName = dp.Product.ProductName,
                ProductType = dp.Product.ProductType,
                ImageURL = dp.Product.ImageURL,
                RiskLevel = dp.Product.RiskLevel.ToString(),
                WarningLabels = dp.Product.WarningLabels.Select(label => new ProductWarningLabelDto
                {
                    Type = label.Type,
                    Description = label.Description
                }).ToList(),
                Dangers = dp.Product.Dangers,
                Precautions = dp.Product.Precautions,
                Alternatives = dp.Product.Alternatives,
                Ingredients = dp.Product.Ingredients.Select(i => new IngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    IsHazardous = i.IsHazardous,
                    Concentration = i.Concentration
                }).ToList()
            } : null
        }).ToList()
    };

}
