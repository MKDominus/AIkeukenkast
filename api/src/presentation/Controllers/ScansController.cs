using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScansController : ControllerBase
{
    private readonly IScanService _service;

    public ScansController(IScanService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScanDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScanDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<ScanDto>> Create(CreateScanDto dto)
    {
        var entity = new Scan 
        { 
            ScanDate = DateTime.UtcNow,
            ImageUrl = dto.ImageUrl,
            MunicipalityId = dto.MunicipalityId,
            UserId = dto.UserId,
            DetectedProducts = dto.DetectedProducts.Select(dp => new DetectedProduct
            {
                ProductId = dp.ProductId,
                Confidence = dp.Confidence,
                Count = dp.Count
            }).ToList()
        };
        
        await _service.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateScanDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        
        entity.ImageUrl = dto.ImageUrl;
        entity.MunicipalityId = dto.MunicipalityId;
        entity.UserId = dto.UserId;
        
        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ScanDto>>> GetByUserId(int userId)
    {
        var entities = await _service.GetScansByUserIdAsync(userId);
        return Ok(entities.Select(ToDto));
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
        UserId = s.UserId,
        User = s.User != null ? new UserDto 
        { 
            Id = s.User.Id, 
            Name = s.User.Name, 
            Age = s.User.Age 
        } : null,
        DetectedProducts = s.DetectedProducts.Select(dp => new DetectedProductDto
        {
            Id = dp.Id,
            ProductId = dp.ProductId,
            Confidence = dp.Confidence,
            Count = dp.Count,
            Product = dp.Product != null ? new ProductDto 
            { 
                Id = dp.Product.Id, 
                Name = dp.Product.Name, 
                Brand = dp.Product.Brand 
            } : null
        }).ToList()
    };
}

