using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

/// <summary>
/// Repository interface for unit likes
/// </summary>
public interface IUnitLikeRepository : IGenericRepository<UnitLike>
{
    /// <summary>
    /// Check if a user has liked a unit
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>True if the user has liked the unit, false otherwise</returns>
    Task<bool> HasUserLikedUnitAsync(Guid userId, Guid unitId);
    
    /// <summary>
    /// Get a specific unit like by user and unit IDs
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>The unit like if it exists, null otherwise</returns>
    Task<UnitLike?> GetByUserAndUnitAsync(Guid userId, Guid unitId);
    
    /// <summary>
    /// Get all units liked by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>List of all unit likes by the user</returns>
    Task<List<UnitLike>> GetByUserAsync(Guid userId);
    
    /// <summary>
    /// Get all likes for a specific unit
    /// </summary>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>List of all likes for the unit</returns>
    Task<List<UnitLike>> GetByUnitAsync(Guid unitId);
    
    /// <summary>
    /// Get the count of likes for a specific unit
    /// </summary>
    /// <param name="unitId">The ID of the unit</param>
    /// <returns>The number of likes</returns>
    Task<int> GetLikesCountAsync(Guid unitId);
} 