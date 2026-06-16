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

    [HttpGet("{id}/alternatives")]
    public async Task<ActionResult<IEnumerable<ProductAlternativesDto>>> GetAllAlternativesForProduct(int id)
    {
        var entities = await _service.GetAllAlternativesForProductAsync(id);
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
            ProductName = dto.ProductName,
            ProductType = dto.ProductType,
            Supplier = dto.Supplier,
            DangerSymbols = dto.DangerSymbols,
            ImageURL = dto.ImageURL,
            RiskLevel = Enum.Parse<ProductRiskLevel>(dto.RiskLevel, ignoreCase: true),
            WarningLabels = dto.WarningLabels.Select(label => new ProductWarningLabel
            {
                Type = label.Type,
                Description = label.Description
            }).ToList(),
            Dangers = dto.Dangers,
            Precautions = dto.Precautions,
            Alternatives = dto.Alternatives,
            Ingredients = ingredients
        };
        
        await _service.AddAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.ProductId }, ToDto(entity));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        if (id != dto.ProductId) return BadRequest();
        
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var ingredients = await _service.GetIngredientsByIdsAsync(dto.IngredientIds ?? new List<int>());
        
        entity.ProductName = dto.ProductName;
        entity.ProductType = dto.ProductType;
        entity.Supplier = dto.Supplier;
        entity.DangerSymbols = dto.DangerSymbols;
        entity.ImageURL = dto.ImageURL;
        entity.RiskLevel = Enum.Parse<ProductRiskLevel>(dto.RiskLevel, ignoreCase: true);
        entity.WarningLabels = dto.WarningLabels.Select(label => new ProductWarningLabel
        {
            Type = label.Type,
            Description = label.Description
        }).ToList();
        entity.Dangers = dto.Dangers;
        entity.Precautions = dto.Precautions;
        entity.Alternatives = dto.Alternatives;
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
        ProductId = p.ProductId,
        ProductName = p.ProductName,
        ProductType = p.ProductType,
        Supplier = p.Supplier,
        DangerSymbols = p.DangerSymbols,
        ImageURL = p.ImageURL,
        RiskLevel = p.RiskLevel.ToString(),
        WarningLabels = p.WarningLabels.Select(label => new ProductWarningLabelDto
        {
            Type = label.Type,
            Description = label.Description
        }).ToList(),
        Dangers = p.Dangers,
        Precautions = p.Precautions,
        Alternatives = p.Alternatives,
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

