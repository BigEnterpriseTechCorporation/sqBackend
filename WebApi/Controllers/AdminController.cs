using System.Security.Claims;
using ApplicationCore.DTOs.Requests;
using ApplicationCore.DTOs.Responses;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApi.Controllers;

/// <summary>
/// Admin controller for system management
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUnitService _unitService;
    private readonly IExerciseService _exerciseService;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Constructor for AdminController
    /// </summary>
    public AdminController(
        IUserService userService,
        IUnitService unitService,
        IExerciseService exerciseService,
        IUserRepository userRepository)
    {
        _userService = userService;
        _unitService = unitService;
        _exerciseService = exerciseService;
        _userRepository = userRepository;
    }

    #region User Management

    /// <summary>
    /// Get all users with filtering and pagination
    /// </summary>
    /// <param name="role">Filter by role (optional)</param>
    /// <param name="searchTerm">Search by username or full name (optional)</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    /// <returns>List of users matching the criteria</returns>
    /// <response code="200">Returns the filtered list of users</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("users")]
    [ProducesResponseType(typeof(PaginatedResponse<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? role = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1)
                page = 1;
                
            if (pageSize < 1 || pageSize > 100)
                pageSize = 20;
                
            var users = await _userService.GetAllUsersAsync();
            
            // Apply filters
            var filteredUsers = users
                .Where(u => string.IsNullOrWhiteSpace(role) || u.Role == role)
                .Where(u => string.IsNullOrWhiteSpace(searchTerm) || 
                            u.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            u.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            // Apply pagination
            var pagedUsers = filteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            // Remove sensitive data
            foreach (var user in pagedUsers)
            {
                user.PasswordHash = null;
            }
            
            return Ok(new PaginatedResponse<User>
            {
                Items = pagedUsers,
                Page = page,
                PageSize = pageSize,
                TotalCount = filteredUsers.Count,
                TotalPages = (int)Math.Ceiling(filteredUsers.Count / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }
    
    /// <summary>
    /// Update a user's role
    /// </summary>
    /// <param name="id">The ID of the user</param>
    /// <param name="request">The role update request</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the role was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("users/{id:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid user ID" });
                
            if (string.IsNullOrWhiteSpace(request.Role))
                return BadRequest(new { message = "Role is required" });
                
            // Validate the role value
            if (request.Role != "Admin" && request.Role != "Member")
                return BadRequest(new { message = "Invalid role. Allowed values: Admin, Member" });
                
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });
                
            // Don't allow changing your own role (to prevent removal of admin access)
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (adminId != null && Guid.Parse(adminId) == id)
                return BadRequest(new { message = "You cannot change your own role" });
                
            user.Role = request.Role;
            await _userService.UpdateUserAsync(user);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user role for user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user role" });
        }
    }
    
    /// <summary>
    /// Toggle a user's active status (activate/deactivate)
    /// </summary>
    /// <param name="id">The ID of the user</param>
    /// <param name="request">The active status update request</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the status was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the user is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("users/{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleUserStatus(Guid id, [FromBody] UpdateActiveStatusRequest request)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid user ID" });
                
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });
                
            // Don't allow deactivating your own account
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (adminId != null && Guid.Parse(adminId) == id && !request.IsActive)
                return BadRequest(new { message = "You cannot deactivate your own account" });
                
            user.IsActive = request.IsActive;
            await _userService.UpdateUserAsync(user);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating active status for user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user status" });
        }
    }

    #endregion

    #region Unit Management

    /// <summary>
    /// Get all units with filtering and pagination
    /// </summary>
    /// <param name="ownerId">Filter by owner ID (optional)</param>
    /// <param name="searchTerm">Search by title or description (optional)</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    /// <returns>List of units matching the criteria</returns>
    /// <response code="200">Returns the filtered list of units</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("units")]
    [ProducesResponseType(typeof(PaginatedResponse<Unit>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUnits(
        [FromQuery] Guid? ownerId = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1)
                page = 1;
                
            if (pageSize < 1 || pageSize > 100)
                pageSize = 20;
                
            var units = await _unitService.GetAllUnitsAsync();
            
            // Apply filters
            var filteredUnits = units
                .Where(u => ownerId == null || u.OwnerId == ownerId)
                .Where(u => string.IsNullOrWhiteSpace(searchTerm) || 
                           u.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           u.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            // Apply pagination
            var pagedUnits = filteredUnits
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return Ok(new PaginatedResponse<Unit>
            {
                Items = pagedUnits,
                Page = page,
                PageSize = pageSize,
                TotalCount = filteredUnits.Count,
                TotalPages = (int)Math.Ceiling(filteredUnits.Count / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving units");
            return StatusCode(500, new { message = "An error occurred while retrieving units" });
        }
    }
    
    /// <summary>
    /// Update a unit's active status
    /// </summary>
    /// <param name="id">The ID of the unit</param>
    /// <param name="request">The active status update request</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the status was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the unit is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("units/{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleUnitStatus(Guid id, [FromBody] UpdateActiveStatusRequest request)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid unit ID" });
                
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
                return NotFound(new { message = "Unit not found" });
                
            unit.IsActive = request.IsActive;
            await _unitService.UpdateUnitAsync(unit);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating active status for unit {UnitId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the unit status" });
        }
    }
    
    /// <summary>
    /// Delete a unit (admin override)
    /// </summary>
    /// <param name="id">The ID of the unit to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the unit was deleted successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the unit is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("units/{id:guid}")]
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
                return BadRequest(new { message = "Invalid unit ID" });
                
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
                return NotFound(new { message = "Unit not found" });
                
            await _unitService.DeleteUnitAsync(id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting unit {UnitId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the unit" });
        }
    }

    #endregion

    #region Exercise Management

    /// <summary>
    /// Get all exercises with filtering and pagination
    /// </summary>
    /// <param name="unitId">Filter by unit ID (optional)</param>
    /// <param name="type">Filter by exercise type (optional)</param>
    /// <param name="difficulty">Filter by difficulty (optional)</param>
    /// <param name="searchTerm">Search by title or description (optional)</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    /// <returns>List of exercises matching the criteria</returns>
    /// <response code="200">Returns the filtered list of exercises</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("exercises")]
    [ProducesResponseType(typeof(PaginatedResponse<Exercise>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExercises(
        [FromQuery] Guid? unitId = null,
        [FromQuery] string? type = null,
        [FromQuery] string? difficulty = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (page < 1)
                page = 1;
                
            if (pageSize < 1 || pageSize > 100)
                pageSize = 20;
                
            var exercises = await _exerciseService.GetAllExercisesAsync();
            
            // Apply filters
            var filteredExercises = exercises
                .Where(e => unitId == null || e.UnitId == unitId)
                .Where(e => string.IsNullOrWhiteSpace(type) || e.Type.ToString() == type)
                .Where(e => string.IsNullOrWhiteSpace(difficulty) || e.Difficulty.ToString() == difficulty)
                .Where(e => string.IsNullOrWhiteSpace(searchTerm) || 
                           e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            // Apply pagination
            var pagedExercises = filteredExercises
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return Ok(new PaginatedResponse<Exercise>
            {
                Items = pagedExercises,
                Page = page,
                PageSize = pageSize,
                TotalCount = filteredExercises.Count,
                TotalPages = (int)Math.Ceiling(filteredExercises.Count / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving exercises");
            return StatusCode(500, new { message = "An error occurred while retrieving exercises" });
        }
    }
    
    /// <summary>
    /// Update an exercise's active status
    /// </summary>
    /// <param name="id">The ID of the exercise</param>
    /// <param name="request">The active status update request</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the status was updated successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the exercise is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("exercises/{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ToggleExerciseStatus(Guid id, [FromBody] UpdateActiveStatusRequest request)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid exercise ID" });
                
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise == null)
                return NotFound(new { message = "Exercise not found" });
                
            exercise.IsActive = request.IsActive;
            await _exerciseService.UpdateExerciseAsync(exercise);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating active status for exercise {ExerciseId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the exercise status" });
        }
    }
    
    /// <summary>
    /// Delete an exercise (admin override)
    /// </summary>
    /// <param name="id">The ID of the exercise to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the exercise was deleted successfully</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="404">If the exercise is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("exercises/{id:guid}")]
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
                return BadRequest(new { message = "Invalid exercise ID" });
                
            var exercise = await _exerciseService.GetExerciseByIdAsync(id);
            if (exercise == null)
                return NotFound(new { message = "Exercise not found" });
                
            await _exerciseService.DeleteExerciseAsync(id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting exercise {ExerciseId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the exercise" });
        }
    }

    #endregion

    #region Statistics

    /// <summary>
    /// Get system statistics
    /// </summary>
    /// <returns>System-wide statistics</returns>
    /// <response code="200">Returns the system statistics</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user is not authorized</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(SystemStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSystemStats()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var units = await _unitService.GetAllUnitsAsync();
            var exercises = await _exerciseService.GetAllExercisesAsync();
            
            var stats = new SystemStatsResponse
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive),
                AdminUsers = users.Count(u => u.Role == "Admin"),
                UsersByCreationDate = users
                    .GroupBy(u => u.CreatedAt.Date)
                    .Select(g => new DateCountPair 
                    { 
                        Date = g.Key, 
                        Count = g.Count() 
                    })
                    .OrderBy(x => x.Date)
                    .ToList(),
                
                TotalUnits = units.Count,
                TotalExercises = exercises.Count,
                ExercisesByType = exercises
                    .GroupBy(e => e.Type)
                    .Select(g => new KeyValuePair<string, int>(
                        g.Key.ToString(), 
                        g.Count()))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                
                ExercisesByDifficulty = exercises
                    .GroupBy(e => e.Difficulty)
                    .Select(g => new KeyValuePair<string, int>(
                        g.Key.ToString(), 
                        g.Count()))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
            
            return Ok(stats);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving system statistics");
            return StatusCode(500, new { message = "An error occurred while retrieving system statistics" });
        }
    }

    #endregion
} 