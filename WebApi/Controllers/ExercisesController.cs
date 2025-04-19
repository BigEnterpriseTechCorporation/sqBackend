using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers;

/// <summary>
/// API endpoints for managing exercises
/// </summary>
[ApiController]
[Route("[controller]")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;
    private readonly IUnitService _unitService;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Constructor for ExercisesController
    /// </summary>
    public ExercisesController(
        IExerciseService exerciseService,
        IUnitService unitService,
        UserManager<User> userManager)
    {
        _exerciseService = exerciseService;
        _unitService = unitService;
        _userManager = userManager;
    }

    /// <summary>
    /// Get all exercises
    /// </summary>
    /// <returns>List of all exercises</returns>
    /// <response code="200">Returns the list of exercises</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetExercises()
    {
        var exercises = await _exerciseService.GetAllExercisesAsync();
        var exerciseDtos = exercises.Select(MapToDto).ToList();
        return Ok(exerciseDtos);
    }

    /// <summary>
    /// Get a specific exercise by ID
    /// </summary>
    /// <param name="id">The ID of the exercise to retrieve</param>
    /// <returns>The exercise with the specified ID</returns>
    /// <response code="200">Returns the exercise</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="404">If the exercise doesn't exist</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseDto>> GetExercise(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid Exercise ID");
        }
        
        var exercise = await _exerciseService.GetExerciseByIdAsync(id);
        
        if (exercise == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(exercise));
    }

    /// <summary>
    /// Create a new exercise
    /// </summary>
    /// <param name="request">The exercise data</param>
    /// <returns>The newly created exercise</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/exercises
    ///     {
    ///        "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///        "title": "Select all customers",
    ///        "description": "Write a query to select all customers",
    ///        "type": "SimpleQuery",
    ///        "difficulty": "Easy",
    ///        "schema": "CREATE TABLE Customers (ID int, Name varchar(255))", # and insert sample values
    ///        "checkType": "Compare",
    ///        "solutionQuery": "SELECT * FROM Customers"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created exercise</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to create an exercise in this unit</response>
    /// <response code="404">If the unit doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ExerciseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ExerciseDto>> CreateExercise([FromBody] CreateExerciseRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("Exercise data is required");
            }
            
            if (request.UnitId == Guid.Empty)
            {
                return BadRequest("Valid Unit ID is required");
            }
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }
            
            if (string.IsNullOrWhiteSpace(request.Schema))
            {
                return BadRequest("Schema is required");
            }
            
            // Validate type-specific fields
            if ((request.Type == ExerciseType.SelectAnswer || request.Type == ExerciseType.FillMissingWords) 
                && string.IsNullOrWhiteSpace(request.Options))
            {
                return BadRequest($"Options are required for {request.Type} exercise type");
            }
            
            if (request.Type == ExerciseType.ConstructQuery && string.IsNullOrWhiteSpace(request.QueryParts))
            {
                return BadRequest("Query parts are required for ConstructQuery exercise type");
            }
            
            var unit = await _unitService.GetUnitByIdAsync(request.UnitId);
            if (unit == null)
            {
                return NotFound($"Unit with ID {request.UnitId} not found");
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

            var exercise = new Exercise
            {
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                Type = request.Type,
                Difficulty = request.Difficulty,
                Schema = request.Schema,
                CheckType = request.CheckType,
                CheckQueryInsert = request.CheckQueryInsert ?? string.Empty,
                CheckQuerySelect = request.CheckQuerySelect ?? string.Empty,
                SolutionQuery = request.SolutionQuery ?? string.Empty,
                Options = request.Options ?? string.Empty,
                QueryParts = request.QueryParts ?? string.Empty
            };

            var createdExercise = await _exerciseService.CreateExerciseAsync(request.UnitId, exercise);
            return CreatedAtAction(nameof(GetExercise), new { id = createdExercise.Id }, MapToDto(createdExercise));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while creating the exercise");
        }
    }

    /// <summary>
    /// Update an existing exercise
    /// </summary>
    /// <param name="id">The ID of the exercise to update</param>
    /// <param name="request">The updated exercise data</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the exercise was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to update this exercise</response>
    /// <response code="404">If the exercise doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateExercise(Guid id, [FromBody] UpdateExerciseRequest request)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Exercise ID");
            }
            
            if (request == null)
            {
                return BadRequest("Exercise data is required");
            }
            
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required");
            }
            
            if (string.IsNullOrWhiteSpace(request.Schema))
            {
                return BadRequest("Schema is required");
            }
            
            // Validate type-specific fields
            if ((request.Type == ExerciseType.SelectAnswer || request.Type == ExerciseType.FillMissingWords) 
                && string.IsNullOrWhiteSpace(request.Options))
            {
                return BadRequest($"Options are required for {request.Type} exercise type");
            }
            
            if (request.Type == ExerciseType.ConstructQuery && string.IsNullOrWhiteSpace(request.QueryParts))
            {
                return BadRequest("Query parts are required for ConstructQuery exercise type");
            }
            
            var existingExercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (existingExercise == null)
            {
                return NotFound();
            }

            var unit = await _unitService.GetUnitByIdAsync(existingExercise.UnitId);
            if (unit == null)
            {
                return NotFound("Associated unit not found");
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

            existingExercise.Title = request.Title;
            existingExercise.Description = request.Description ?? string.Empty;
            existingExercise.Type = request.Type;
            existingExercise.Difficulty = request.Difficulty;
            existingExercise.Schema = request.Schema;
            existingExercise.CheckType = request.CheckType;
            existingExercise.CheckQueryInsert = request.CheckQueryInsert ?? string.Empty;
            existingExercise.CheckQuerySelect = request.CheckQuerySelect ?? string.Empty;
            existingExercise.SolutionQuery = request.SolutionQuery ?? string.Empty;
            existingExercise.Options = request.Options ?? string.Empty;
            existingExercise.QueryParts = request.QueryParts ?? string.Empty;

            await _exerciseService.UpdateExerciseAsync(existingExercise);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while updating the exercise");
        }
    }

    /// <summary>
    /// Delete an exercise
    /// </summary>
    /// <param name="id">The ID of the exercise to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the exercise was deleted successfully</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized to delete this exercise</response>
    /// <response code="404">If the exercise doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteExercise(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Exercise ID");
            }
            
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }

            var unit = await _unitService.GetUnitByIdAsync(exercise.UnitId);
            if (unit == null)
            {
                return NotFound("Associated unit not found");
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

            await _exerciseService.DeleteExerciseAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while deleting the exercise");
        }
    }

    private ExerciseDto MapToDto(Exercise exercise)
    {
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }
        
        return new ExerciseDto
        {
            Id = exercise.Id,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt,
            UnitId = exercise.UnitId,
            UnitTitle = exercise.Unit?.Title ?? string.Empty,
            Title = exercise.Title,
            Description = exercise.Description,
            Type = exercise.Type,
            Difficulty = exercise.Difficulty,
            Schema = exercise.Schema,
            CheckType = exercise.CheckType,
            CheckQueryInsert = exercise.CheckQueryInsert,
            CheckQuerySelect = exercise.CheckQuerySelect,
            SolutionQuery = exercise.SolutionQuery,
            Options = exercise.Options,
            QueryParts = exercise.QueryParts
        };
    }
} 