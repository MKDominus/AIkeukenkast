using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var entities = await _service.GetAllAsync();
        var dtos = entities.Select(e => new UserDto 
        { 
            Id = e.Id, 
            Name = e.Name, 
            Age = e.Age 
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        
        return Ok(new UserDto 
        { 
            Id = item.Id, 
            Name = item.Name, 
            Age = item.Age 
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        var entity = new User 
        { 
            Name = dto.Name, 
            Age = dto.Age 
        };
        
        await _service.AddAsync(entity);
        
        var returnDto = new UserDto 
        { 
            Id = entity.Id, 
            Name = entity.Name, 
            Age = entity.Age 
        };
        
        return CreatedAtAction(nameof(GetById), new { id = returnDto.Id }, returnDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto dto)
    {
        if (id != dto.Id) return BadRequest();
        
        var entity = new User 
        { 
            Id = dto.Id, 
            Name = dto.Name, 
            Age = dto.Age 
        };
        
        await _service.UpdateAsync(entity);
        return NoContent();
    }

    // Delete remains the same
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}

