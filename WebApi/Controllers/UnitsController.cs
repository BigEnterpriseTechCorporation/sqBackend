using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

/// <summary>
/// API endpoints for managing units (collections of exercises)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;
    private readonly IExerciseService _exerciseService;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Constructor for UnitsController
    /// </summary>
    public UnitsController(
        IUnitService unitService,
        IExerciseService exerciseService,
        UserManager<User> userManager)
    {
        _unitService = unitService;
        _exerciseService = exerciseService;
        _userManager = userManager;
    }

    /// <summary>
    /// Get all units
    /// </summary>
    /// <returns>List of all units</returns>
    /// <response code="200">Returns the list of units</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UnitDto>>> GetUnits()
    {
        try
        {
            var units = await _unitService.GetAllUnitsAsync();
            var unitDtos = units.Select(MapToDto).ToList();
            return Ok(unitDtos);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while retrieving units");
        }
    }

    /// <summary>
    /// Get a specific unit by ID with all its exercises
    /// </summary>
    /// <param name="id">The ID of the unit to retrieve</param>
    /// <returns>The unit with the specified ID</returns>
    /// <response code="200">Returns the unit</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="404">If the unit doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UnitDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UnitDetailDto>> GetUnit(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Unit ID");
            }
            
            var unit = await _unitService.GetUnitByIdAsync(id);
            
            if (unit == null)
            {
                return NotFound();
            }

            return Ok(MapToDetailDto(unit));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while retrieving the unit");
        }
    }

    /// <summary>
    /// Get all exercises for a specific unit
    /// </summary>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>List of exercises in the unit</returns>
    /// <response code="200">Returns the list of exercises</response>
    /// <response code="400">If the unit ID is invalid</response>
    /// <response code="404">If the unit doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{unitId:guid}/exercises")]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetUnitExercises(Guid unitId)
    {
        try
        {
            if (unitId == Guid.Empty)
            {
                return BadRequest("Invalid Unit ID");
            }
            
            var unit = await _unitService.GetUnitByIdAsync(unitId);
            if (unit == null)
            {
                return NotFound();
            }

            var exercises = await _unitService.GetAllExercisesAsync(unitId);
            var exerciseDtos = exercises.Select(e => new ExerciseDto
            {
                Id = e.Id,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                UnitId = e.UnitId,
                UnitTitle = unit.Title,
                Title = e.Title,
                Description = e.Description,
                Type = e.Type,
                Difficulty = e.Difficulty,
                Schema = e.Schema,
                CheckType = e.CheckType,
                CheckQueryInsert = e.CheckQueryInsert,
                CheckQuerySelect = e.CheckQuerySelect,
                Options = e.Options,
                QueryParts = e.QueryParts
            }).ToList();
            
            return Ok(exerciseDtos);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while retrieving exercises");
        }
    }

    /// <summary>
    /// Create a new unit
    /// </summary>
    /// <param name="request">The unit data</param>
    /// <returns>The newly created unit</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/units
    ///     {
    ///        "title": "SQL Basics",
    ///        "description": "Learn the basics of SQL"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created unit</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UnitDto>> CreateUnit([FromBody] CreateUnitRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Unit data is required");
            }
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var unit = await _unitService.CreateUnitAsync(currentUser, request.Title, request.Description);
            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, MapToDto(unit));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while creating the unit");
        }
    }

    /// <summary>
    /// Update an existing unit
    /// </summary>
    /// <param name="id">The ID of the unit to update</param>
    /// <param name="request">The updated unit data</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the unit was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this unit</response>
    /// <response code="404">If the unit doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUnit(Guid id, [FromBody] UpdateUnitRequest request)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Unit ID");
            }
            
            if (request == null)
            {
                return BadRequest("Unit data is required");
            }
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }
            
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            
            if (unit.OwnerId != currentUser.Id)
            {
                return Forbid();
            }

            unit.Title = request.Title;
            unit.Description = request.Description ?? string.Empty;
            
            await _unitService.UpdateUnitAsync(unit);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while updating the unit");
        }
    }

    /// <summary>
    /// Delete a unit
    /// </summary>
    /// <param name="id">The ID of the unit to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the unit was deleted successfully</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to delete this unit</response>
    /// <response code="404">If the unit doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUnit(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Unit ID");
            }
            
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            
            if (unit.OwnerId != currentUser.Id)
            {
                return Forbid();
            }

            await _unitService.DeleteUnitAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while deleting the unit");
        }
    }

    private UnitDto MapToDto(Unit unit)
    {
        if (unit == null)
        {
            throw new ArgumentNullException(nameof(unit));
        }
        
        return new UnitDto
        {
            Id = unit.Id,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt,
            Title = unit.Title,
            Description = unit.Description,
            OwnerId = unit.OwnerId,
            OwnerName = unit.Owner?.FullName ?? string.Empty,
            ExerciseCount = unit.Exercises?.Count ?? 0
        };
    }

    private UnitDetailDto MapToDetailDto(Unit unit)
    {
        if (unit == null)
        {
            throw new ArgumentNullException(nameof(unit));
        }
        
        var dto = new UnitDetailDto
        {
            Id = unit.Id,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt,
            Title = unit.Title,
            Description = unit.Description,
            OwnerId = unit.OwnerId,
            OwnerName = unit.Owner?.FullName ?? string.Empty,
            ExerciseCount = unit.Exercises?.Count ?? 0
        };

        if (unit.Exercises != null)
        {
            dto.Exercises = unit.Exercises
                .Where(e => e.IsActive)
                .Select(e => new ExerciseDto
                {
                    Id = e.Id,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    UnitId = e.UnitId,
                    UnitTitle = unit.Title,
                    Title = e.Title,
                    Description = e.Description,
                    Type = e.Type,
                    Difficulty = e.Difficulty,
                    Schema = e.Schema,
                    CheckType = e.CheckType,
                    CheckQueryInsert = e.CheckQueryInsert,
                    CheckQuerySelect = e.CheckQuerySelect,
                    Options = e.Options,
                    QueryParts = e.QueryParts
                }).ToList();
        }

        return dto;
    }
}

/// <summary>
/// Data model for creating a new unit
/// </summary>
public class CreateUnitRequest
{
    /// <summary>
    /// Title of the unit
    /// </summary>
    /// <example>SQL Basics</example>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the unit
    /// </summary>
    /// <example>Learn the basics of SQL including SELECT, INSERT, UPDATE, DELETE</example>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Data model for updating an existing unit
/// </summary>
public class UpdateUnitRequest
{
    /// <summary>
    /// Title of the unit
    /// </summary>
    /// <example>SQL Basics</example>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the unit
    /// </summary>
    /// <example>Learn the basics of SQL including SELECT, INSERT, UPDATE, DELETE</example>
    public string Description { get; set; } = string.Empty;
} 