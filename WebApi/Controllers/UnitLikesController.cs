using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

/// <summary>
/// Controller for handling unit likes
/// </summary>
[ApiController]
[Route("units/{unitId}/likes")]
[Authorize]
public class UnitLikesController : ControllerBase
{
    private readonly IUnitLikeRepository _unitLikeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGenericRepository<Unit> _unitRepository;
    
    public UnitLikesController(
        IUnitLikeRepository unitLikeRepository,
        IUserRepository userRepository,
        IGenericRepository<Unit> unitRepository)
    {
        _unitLikeRepository = unitLikeRepository;
        _userRepository = userRepository;
        _unitRepository = unitRepository;
    }
    
    /// <summary>
    /// Like or unlike a unit
    /// </summary>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>The result of the like operation</returns>
    [HttpPost]
    public async Task<ActionResult<LikeResult>> ToggleLike(Guid unitId)
    {
        // Get the current user's ID from the token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("User ID not found in token or invalid format");
        }
        
        try
        {
            // Check if the unit exists
            var unit = await _unitRepository.GetByIdAsync(unitId);
            if (unit == null)
            {
                return NotFound($"Unit with ID {unitId} not found");
            }
            
            // Check if the user already liked this unit
            var hasLiked = await _unitLikeRepository.HasUserLikedUnitAsync(userId, unitId);
            
            if (hasLiked)
            {
                // Unlike the unit
                var like = await _unitLikeRepository.GetByUserAndUnitAsync(userId, unitId);
                if (like != null)
                {
                    await _unitLikeRepository.DeleteAsync(like.Id);
                    
                    // Update counts
                    unit.LikesCount = Math.Max(0, unit.LikesCount - 1);
                    await _unitRepository.UpdateAsync(unit);
                    
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null)
                    {
                        user.LikedUnitsCount = Math.Max(0, user.LikedUnitsCount - 1);
                        await _userRepository.UpdateAsync(user);
                    }
                    
                    return Ok(new LikeResult
                    {
                        IsLiked = false,
                        LikesCount = unit.LikesCount
                    });
                }
            }
            else
            {
                // Like the unit
                var like = new UnitLike
                {
                    UserId = userId,
                    UnitId = unitId,
                    LikedAt = DateTime.UtcNow
                };
                
                await _unitLikeRepository.AddAsync(like);
                
                // Update counts
                unit.LikesCount++;
                await _unitRepository.UpdateAsync(unit);
                
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.LikedUnitsCount++;
                    await _userRepository.UpdateAsync(user);
                }
                
                return Ok(new LikeResult
                {
                    IsLiked = true,
                    LikesCount = unit.LikesCount
                });
            }
            
            return BadRequest("Failed to toggle like status");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Check if the current user has liked a unit
    /// </summary>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>The like status</returns>
    [HttpGet]
    public async Task<ActionResult<LikeResult>> GetLikeStatus(Guid unitId)
    {
        // Get the current user's ID from the token claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return Unauthorized("User ID not found in token or invalid format");
        }
        
        try
        {
            // Check if the unit exists
            var unit = await _unitRepository.GetByIdAsync(unitId);
            if (unit == null)
            {
                return NotFound($"Unit with ID {unitId} not found");
            }
            
            // Check if the user already liked this unit
            var hasLiked = await _unitLikeRepository.HasUserLikedUnitAsync(userId, unitId);
            
            return Ok(new LikeResult
            {
                IsLiked = hasLiked,
                LikesCount = unit.LikesCount
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Result model for like operations
/// </summary>
public class LikeResult
{
    /// <summary>
    /// Whether the user has liked the unit
    /// </summary>
    public bool IsLiked { get; set; }
    
    /// <summary>
    /// Total number of likes for the unit
    /// </summary>
    public int LikesCount { get; set; }
} 