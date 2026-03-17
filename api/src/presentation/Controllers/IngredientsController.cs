using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _service;

    public IngredientsController(IIngredientService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<IngredientDto>> Create(CreateIngredientDto dto)
    {
        var entity = new Ingredient 
        { 
            Name = dto.Name,
            Description = dto.Description,
            IsHazardous = dto.IsHazardous,
            Concentration = dto.Concentration
        };
        
        await _service.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateIngredientDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = new Ingredient 
        { 
            Id = dto.Id, 
            Name = dto.Name,
            Description = dto.Description,
            IsHazardous = dto.IsHazardous,
            Concentration = dto.Concentration
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
    
    private static IngredientDto ToDto(Ingredient i) => new IngredientDto
    {
        Id = i.Id,
        Name = i.Name,
        Description = i.Description,
        IsHazardous = i.IsHazardous,
        Concentration = i.Concentration
    };
}

