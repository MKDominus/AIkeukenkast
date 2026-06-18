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
    private readonly IBlobStorageService _blobStorageService;

    public ScansController(
        IScanService service,
        IScanImportService scanImportService,
        IBlobStorageService blobStorageService)
    {
        _service = service;
        _scanImportService = scanImportService;
        _blobStorageService = blobStorageService;
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
            var createdScans = await _scanImportService.CreateFromImagesAsync(
                dto.Images,
                dto.PostalCode,
                dto.PostalCodePermission);
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
        entity.PostalCodePermission = dto.PostalCodePermission;
        entity.PostalCode = dto.PostalCodePermission
            ? string.IsNullOrWhiteSpace(dto.PostalCode) ? null : dto.PostalCode.Trim()
            : null;
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

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<ProductCategoryCountDto>>> GetCategoryCounts()
    {
        var categories = await _service.GetCategoryCountsAsync();
        return Ok(categories);
    }

    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var scan = await _service.GetByIdAsync(id);
        if (scan == null || string.IsNullOrWhiteSpace(scan.ImageUrl))
        {
            return NotFound();
        }

        var image = await _blobStorageService.DownloadImageAsync(scan.ImageUrl);
        if (image == null)
        {
            return NotFound();
        }

        return File(image.Value.Content, image.Value.ContentType);
    }

}
