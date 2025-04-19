using ApplicationCore.DTOs;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Services;

/// <summary>
/// Service for handling exercises and their solutions
/// </summary>
public interface IExerciseService
{
    // Original CRUD operations
    Task<Exercise> CreateExerciseAsync(Guid unitId, Exercise exercise);
    Task<Exercise?> GetExerciseByIdAsync(Guid id);
    Task<List<Exercise>> GetAllExercisesAsync();
    Task<Exercise> UpdateExerciseAsync(Exercise exercise);
    Task DeleteExerciseAsync(Guid id);
    
    // New solution-checking operations
    /// <summary>
    /// Check if a user's solution to an exercise is correct
    /// </summary>
    Task<bool> CheckSolutionAsync(Guid exerciseId, Guid userId, string submittedQuery);
    
    /// <summary>
    /// Submit a solution for an exercise
    /// </summary>
    Task<SolutionResultDto> SubmitSolutionAsync(Guid exerciseId, Guid userId, string submittedQuery);
    
    /// <summary>
    /// Get exercise statistics for a user
    /// </summary>
    Task<UserExerciseStatsDto> GetUserExerciseStatsAsync(Guid userId);
    
    /// <summary>
    /// Get a list of exercises solved by a user
    /// </summary>
    Task<List<ExerciseDto>> GetSolvedExercisesAsync(Guid userId);
    
    /// <summary>
    /// Get a list of exercises not yet solved by a user
    /// </summary>
    Task<List<ExerciseDto>> GetUnsolvedExercisesAsync(Guid userId);
} 