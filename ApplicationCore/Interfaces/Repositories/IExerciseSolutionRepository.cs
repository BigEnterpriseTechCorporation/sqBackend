using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

/// <summary>
/// Repository interface for exercise solutions
/// </summary>
public interface IExerciseSolutionRepository : IGenericRepository<ExerciseSolution>
{
    /// <summary>
    /// Get a user's solution for a specific exercise
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>The solution if it exists, null otherwise</returns>
    Task<ExerciseSolution?> GetUserSolutionAsync(Guid userId, Guid exerciseId);
    
    /// <summary>
    /// Get all solutions for a specific exercise
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>List of all solutions for the exercise</returns>
    Task<List<ExerciseSolution>> GetSolutionsByExerciseAsync(Guid exerciseId);
    
    /// <summary>
    /// Get all solutions by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>List of all solutions by the user</returns>
    Task<List<ExerciseSolution>> GetSolutionsByUserAsync(Guid userId);
    
    /// <summary>
    /// Get only correct solutions by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>List of correct solutions by the user</returns>
    Task<List<ExerciseSolution>> GetCorrectSolutionsByUserAsync(Guid userId);
    
    /// <summary>
    /// Check if a user has correctly solved an exercise
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>True if the user has solved the exercise, false otherwise</returns>
    Task<bool> HasUserSolvedExerciseAsync(Guid userId, Guid exerciseId);
    
    /// <summary>
    /// Get the count of attempts made by a user on an exercise
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="exerciseId">The ID of the exercise</param>
    /// <returns>The number of attempts</returns>
    Task<int> GetAttemptCountAsync(Guid userId, Guid exerciseId);
} 