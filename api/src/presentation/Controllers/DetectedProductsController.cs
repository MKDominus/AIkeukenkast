using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetectedProductsController : ControllerBase
{
    private readonly IDetectedProductService _service;

    public DetectedProductsController(IDetectedProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetectedProductDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DetectedProductDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<DetectedProductDto>> Create(CreateDetectedProductDto dto)
    {
        var entity = new DetectedProduct 
        { 
            ScanId = dto.ScanId,
            ProductId = dto.ProductId,
            Confidence = dto.Confidence,
            Count = dto.Count
        };
        
        await _service.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDetectedProductDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
            
        entity.Confidence = dto.Confidence;
        entity.Count = dto.Count;
        
        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("scan/{scanId}")]
    public async Task<ActionResult<IEnumerable<DetectedProductDto>>> GetByScanId(int scanId)
    {
        var entities = await _service.GetByScanIdAsync(scanId);
        return Ok(entities.Select(ToDto));
    }

    private static DetectedProductDto ToDto(DetectedProduct dp) => new DetectedProductDto
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
            Alternatives = dp.Product.Alternatives
        } : null
    };
}

