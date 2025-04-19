using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApi.Controllers;

/// <summary>
/// Controller for managing users in the system
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Constructor for UsersController
    /// </summary>
    /// <param name="userService">The user service dependency</param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users in the system
    /// </summary>
    /// <returns>List of all users</returns>
    /// <response code="200">Returns the list of users</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have sufficient permissions</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            
            // Remove sensitive information
            foreach (var user in users)
            {
                user.PasswordHash = null;
            }
            
            return Ok(users);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving all users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }

    /// <summary>
    /// Get a specific user by ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The user with the specified ID</returns>
    /// <response code="200">Returns the user</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="404">If the user doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Remove sensitive information
            user.PasswordHash = null;
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving user with ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the user" });
        }
    }
    
    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the user was deleted successfully</response>
    /// <response code="400">If the ID is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user doesn't have sufficient permissions</response>
    /// <response code="404">If the user doesn't exist</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }
            
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting user with ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the user" });
        }
    }
}