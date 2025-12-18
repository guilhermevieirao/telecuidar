using Application.DTOs.Specialties;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SpecialtiesController : ControllerBase
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtiesController(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedSpecialtiesDto>> GetSpecialties(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var result = await _specialtyService.GetSpecialtiesAsync(page, pageSize, search, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SpecialtyDto>> GetSpecialty(Guid id)
    {
        try
        {
            var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (specialty == null)
            {
                return NotFound(new { message = "Specialty not found" });
            }
            return Ok(specialty);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<SpecialtyDto>> CreateSpecialty([FromBody] CreateSpecialtyDto dto)
    {
        try
        {
            var specialty = await _specialtyService.CreateSpecialtyAsync(dto);
            return CreatedAtAction(nameof(GetSpecialty), new { id = specialty.Id }, specialty);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<SpecialtyDto>> UpdateSpecialty(Guid id, [FromBody] UpdateSpecialtyDto dto)
    {
        try
        {
            var specialty = await _specialtyService.UpdateSpecialtyAsync(id, dto);
            if (specialty == null)
            {
                return NotFound(new { message = "Specialty not found" });
            }
            return Ok(specialty);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteSpecialty(Guid id)
    {
        try
        {
            var result = await _specialtyService.DeleteSpecialtyAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Specialty not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }
}
