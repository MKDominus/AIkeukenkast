using api.Application.DTOs;
using api.Application.Interfaces;
using api.Application.Mappers;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScansController : ControllerBase
{
    private readonly IScanService _service;
    private readonly IScanImportService _scanImportService;

    public ScansController(IScanService service, IScanImportService scanImportService)
    {
        _service = service;
        _scanImportService = scanImportService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScanDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ScanMapper.ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScanDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ScanMapper.ToDto(item));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<IEnumerable<ScanDto>>> Create([FromForm] CreateScanFormDto dto)
    {
        try
        {
            var createdScans = await _scanImportService.CreateFromImagesAsync(dto.Images);
            return Ok(createdScans.Select(ScanMapper.ToDto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
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

}
