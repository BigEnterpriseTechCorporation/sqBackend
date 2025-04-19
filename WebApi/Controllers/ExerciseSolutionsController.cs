using ApplicationCore.DTOs;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

/// <summary>
/// Controller for handling exercise solutions
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExerciseSolutionsController : ControllerBase
{
    private readonly IExerciseService _exerciseService;
    
    public ExerciseSolutionsController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }
    
    /// <summary>
    /// Submit a solution for an exercise
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <param name="request">The solution submission request</param>
    /// <returns>The result of the solution check</returns>
    [HttpPost("{exerciseId}")]
    public async Task<ActionResult<SolutionResultDto>> SubmitSolution(Guid exerciseId, [FromBody] SubmitSolutionRequest request)
    {
        // Get the current user's ID from the token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("User ID not found in token or invalid format");
        }
        
        try
        {
            var result = await _exerciseService.SubmitSolutionAsync(exerciseId, userId, request.Query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Get exercise statistics for the current user
    /// </summary>
    /// <returns>User's exercise statistics</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<UserExerciseStatsDto>> GetUserStats()
    {
        // Get the current user's ID from the token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("User ID not found in token or invalid format");
        }
        
        try
        {
            var stats = await _exerciseService.GetUserExerciseStatsAsync(userId);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Get a list of exercises solved by the current user
    /// </summary>
    /// <returns>List of solved exercises</returns>
    [HttpGet("solved")]
    public async Task<ActionResult<List<ExerciseDto>>> GetSolvedExercises()
    {
        // Get the current user's ID from the token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("User ID not found in token or invalid format");
        }
        
        try
        {
            var exercises = await _exerciseService.GetSolvedExercisesAsync(userId);
            return Ok(exercises);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Get a list of exercises not yet solved by the current user
    /// </summary>
    /// <returns>List of unsolved exercises</returns>
    [HttpGet("unsolved")]
    public async Task<ActionResult<List<ExerciseDto>>> GetUnsolvedExercises()
    {
        // Get the current user's ID from the token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("User ID not found in token or invalid format");
        }
        
        try
        {
            var exercises = await _exerciseService.GetUnsolvedExercisesAsync(userId);
            return Ok(exercises);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for submitting a solution
/// </summary>
public class SubmitSolutionRequest
{
    /// <summary>
    /// The SQL query solution
    /// </summary>
    public string Query { get; set; } = string.Empty;
} 