using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        return Ok(entities.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(ToDto(item));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var ingredients = await _service.GetIngredientsByIdsAsync(dto.IngredientIds ?? new List<int>());

        var entity = new Product 
        { 
            Name = dto.Name,
            Brand = dto.Brand,
            ImageUrl = dto.ImageUrl,
            Category = dto.Category,
            SustainabilityScore = dto.SustainabilityScore,
            IsSustainable = dto.IsSustainable,
            SafetyWarnings = dto.SafetyWarnings,
            Ingredients = ingredients
        };
        
        await _service.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var ingredients = await _service.GetIngredientsByIdsAsync(dto.IngredientIds ?? new List<int>());
        
        entity.Name = dto.Name;
        entity.Brand = dto.Brand;
        entity.ImageUrl = dto.ImageUrl;
        entity.Category = dto.Category;
        entity.SustainabilityScore = dto.SustainabilityScore;
        entity.IsSustainable = dto.IsSustainable;
        entity.SafetyWarnings = dto.SafetyWarnings;
        entity.Ingredients = ingredients;
        
        await _service.UpdateAsync(entity);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/ingredients")]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredients(int id)
    {
        var ingredients = await _service.GetIngredientsByProductIdAsync(id);
        return Ok(ingredients.Select(i => new IngredientDto
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            IsHazardous = i.IsHazardous,
            Concentration = i.Concentration
        }));
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<ProductCategoryCountDto>>> GetCategoryCounts()
    {
        var categories = await _service.GetCategoryCountsAsync();
        return Ok(categories);
    }
    
    private static ProductDto ToDto(Product p) => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Brand = p.Brand,
        ImageUrl = p.ImageUrl,
        Category = p.Category,
        SustainabilityScore = p.SustainabilityScore,
        IsSustainable = p.IsSustainable,
        SafetyWarnings = p.SafetyWarnings,
        Ingredients = p.Ingredients.Select(i => new IngredientDto
        {
            Id = i.Id,
            Name = i.Name,
            Description = i.Description,
            IsHazardous = i.IsHazardous,
            Concentration = i.Concentration
        }).ToList()
    };
}

