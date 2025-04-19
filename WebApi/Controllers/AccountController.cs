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
/// Controller for managing user accounts and authentication
/// </summary>
[Route("[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Constructor for AccountController
    /// </summary>
    public AccountController(IUserService userService, ITokenService tokenService, IUserRepository userRepository)
    {
        _userService = userService;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <returns>The newly created user with authentication token</returns>
    /// <response code="201">Returns the newly created user and token</response>
    /// <response code="400">If the registration request is invalid or username already exists</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest(new { message = "Username is required" });
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return BadRequest(new { message = "Password must be at least 8 characters" });
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new { message = "Full name is required" });
        }

        var user = new User
        {
            UserName = request.UserName.ToLower(),
            Email = $"{request.UserName}@localhost",
            FullName = request.FullName,
        };
        
        try
        {
            var createdUser = await _userService.RegisterUserAsync(user, request.Password);
            var response = new RegisterResponse
            {
                Id = createdUser.Id,
                Role = createdUser.Role,
                Token = _tokenService.GenerateJwtToken(createdUser)
            };
            return CreatedAtAction(nameof(GetSelf), null, response);
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error during registration");
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Authenticate a user and get a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>User information and authentication token</returns>
    /// <response code="200">Returns the user and valid token</response>
    /// <response code="401">If the credentials are invalid</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
        {
            return BadRequest(new { message = "Username and password are required" });
        }

        try
        {
            var isValid = await _userService.ValidateCredentialsAsync(request.username, request.password);
            if (!isValid)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            var user = await _userRepository.GetByUsernameAsync(request.username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            return Ok(new LoginResponse
            {
                Id = user.Id,
                Role = user.Role,
                Token = _tokenService.GenerateJwtToken(user)
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Login error for user {Username}", request.username);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Get a new JWT token for an authenticated user
    /// </summary>
    /// <returns>A new authentication token</returns>
    /// <response code="200">Returns a new token</response>
    /// <response code="400">If the token request is invalid</response>
    /// <response code="401">If the user is not authenticated or not found</response>
    [HttpGet("refresh")]
    [Authorize]
    [ProducesResponseType(typeof(RefreshResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRefreshToken()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user identifier in token" });
            }
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }
            
            return Ok(new RefreshResponse
            {
                Token = _tokenService.GenerateJwtToken(user)
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error refreshing token");
            return StatusCode(500, new { message = "An error occurred while refreshing the token" });
        }
    }

    /// <summary>
    /// Get the currently authenticated user's information
    /// </summary>
    /// <returns>Current user information</returns>
    /// <response code="200">Returns the current user</response>
    /// <response code="400">If the user identifier is invalid</response>
    /// <response code="401">If the user is not authenticated or not found</response>
    [HttpGet("self")]
    [Authorize]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSelf()
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user identifier in token" });
            }
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }
            
            // Don't return password hash to client
            user.PasswordHash = null;
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving user information");
            return StatusCode(500, new { message = "An error occurred while retrieving user information" });
        }
    }
    
    /// <summary>
    /// Update the currently authenticated user's profile information
    /// </summary>
    /// <param name="request">The profile update information</param>
    /// <returns>The updated user profile</returns>
    /// <response code="200">Returns the updated user information</response>
    /// <response code="400">If the request is invalid or validation fails</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="404">If the user is not found</response>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "Invalid user identifier in token" });
            }
            
            // At least one update field must be provided
            if (string.IsNullOrWhiteSpace(request.UserName) && 
                string.IsNullOrWhiteSpace(request.FullName) && 
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { message = "At least one field (username, full name, or password) must be provided for update" });
            }
            
            try
            {
                var updatedUser = await _userService.UpdateUserProfileAsync(
                    userId,
                    request.CurrentPassword,
                    request.UserName,
                    request.FullName,
                    request.NewPassword);
                
                // Generate a new token if username or password changed
                var needsNewToken = !string.IsNullOrWhiteSpace(request.UserName) || !string.IsNullOrWhiteSpace(request.NewPassword);
                if (needsNewToken)
                {
                    return Ok(new
                    {
                        user = updatedUser,
                        token = _tokenService.GenerateJwtToken(updatedUser)
                    });
                }
                
                return Ok(updatedUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user profile");
            return StatusCode(500, new { message = "An error occurred while updating the user profile" });
        }
    }
    
    /// <summary>
    /// Extract the user ID from the claims principal
    /// </summary>
    private Guid GetUserIdFromClaims()
    {
        var idClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (idClaim == null || !Guid.TryParseExact(idClaim.Value, "D", out var userId))
        {
            return Guid.Empty;
        }
        return userId;
    }
}