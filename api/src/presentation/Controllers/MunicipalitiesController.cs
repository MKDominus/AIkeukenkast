using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MunicipalitiesController : ControllerBase
{
    private readonly IMunicipalityService _service;

    public MunicipalitiesController(IMunicipalityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MunicipalityDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ToDto));
    }

    [HttpGet("scan-counts")]
    public async Task<ActionResult<IEnumerable<MunicipalityScanCountDto>>> GetScanCounts()
    {
        var counts = await _service.GetScanCountsAsync();
        return Ok(counts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MunicipalityDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<MunicipalityDto>> Create(CreateMunicipalityDto dto)
    {
        var entity = new Municipality 
        { 
            Name = dto.Name, 
            Population = dto.Population 
        };
        
        await _service.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateMunicipalityDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = new Municipality 
        { 
            Id = dto.Id, 
            Name = dto.Name, 
            Population = dto.Population 
        };
        
        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
    
    private static MunicipalityDto ToDto(Municipality m) => new MunicipalityDto
    {
        Id = m.Id,
        Name = m.Name,
        Population = m.Population
    };
}

