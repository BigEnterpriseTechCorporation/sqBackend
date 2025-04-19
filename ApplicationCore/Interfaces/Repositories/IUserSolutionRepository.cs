using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

/// <summary>
/// Repository interface for user solutions
/// </summary>
public interface IUserSolutionRepository : IGenericRepository<UserSolution>
{
    /// <summary>
    /// Get a user's solution for a specific exercise
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>The user's solution if it exists, null otherwise</returns>
    Task<UserSolution?> GetUserSolutionAsync(Guid userId, Guid exerciseId);
    
    /// <summary>
    /// Get all solutions for a specific exercise
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>List of all solutions for the exercise</returns>
    Task<List<UserSolution>> GetSolutionsByExerciseAsync(Guid exerciseId);
    
    /// <summary>
    /// Get all solutions by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>List of all solutions by the user</returns>
    Task<List<UserSolution>> GetSolutionsByUserAsync(Guid userId);
    
    /// <summary>
    /// Get all correct solutions by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>List of all correct solutions by the user</returns>
    Task<List<UserSolution>> GetCorrectSolutionsByUserAsync(Guid userId);
    
    /// <summary>
    /// Check if a user has already solved an exercise
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>True if the user has a correct solution, false otherwise</returns>
    Task<bool> HasUserSolvedExerciseAsync(Guid userId, Guid exerciseId);
    
    /// <summary>
    /// Get the number of attempts a user has made on an exercise
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>The number of attempts</returns>
    Task<int> GetAttemptCountAsync(Guid userId, Guid exerciseId);
} 