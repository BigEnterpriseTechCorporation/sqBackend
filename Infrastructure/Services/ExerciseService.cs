using ApplicationCore.DTOs;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Transactions;

namespace Infrastructure.Services;

/// <summary>
/// Service for handling exercises and solutions
/// </summary>
public class ExerciseService : IExerciseService
{
    private readonly IGenericRepository<Exercise> _exerciseRepository;
    private readonly IUserSolutionRepository _userSolutionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISqlExecutionService _sqlExecutionService;
    private readonly IGenericRepository<Unit> _unitRepository;
    
    public ExerciseService(
        IGenericRepository<Exercise> exerciseRepository,
        IUserSolutionRepository userSolutionRepository,
        IUserRepository userRepository,
        ISqlExecutionService sqlExecutionService,
        IGenericRepository<Unit> unitRepository)
    {
        _exerciseRepository = exerciseRepository;
        _userSolutionRepository = userSolutionRepository;
        _userRepository = userRepository;
        _sqlExecutionService = sqlExecutionService;
        _unitRepository = unitRepository;
    }
    
    /// <summary>
    /// Create a new exercise
    /// </summary>
    public async Task<Exercise> CreateExerciseAsync(Guid unitId, Exercise exercise)
    {
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }
        
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        try
        {
            var unit = await _unitRepository.GetByIdAsync(unitId);
            if (unit == null)
            {
                throw new ArgumentException($"Unit with ID {unitId} not found");
            }
            
            exercise.UnitId = unitId;
            exercise.CreatedAt = DateTime.UtcNow;
            exercise.IsActive = true;
            
            var result = await _exerciseRepository.AddAsync(exercise);
            
            // Update unit references
            unit.UpdatedAt = DateTime.UtcNow;
            await _unitRepository.UpdateAsync(unit);
            
            scope.Complete();
            return result;
        }
        catch
        {
            // Transaction will automatically be rolled back
            throw;
        }
    }
    
    /// <summary>
    /// Get an exercise by ID
    /// </summary>
    public async Task<Exercise?> GetExerciseByIdAsync(Guid id)
    {
        return await _exerciseRepository.GetByIdAsync(id);
    }
    
    /// <summary>
    /// Get all exercises
    /// </summary>
    public async Task<List<Exercise>> GetAllExercisesAsync()
    {
        return await _exerciseRepository.GetAllAsync();
    }
    
    /// <summary>
    /// Update an exercise
    /// </summary>
    public async Task<Exercise> UpdateExerciseAsync(Exercise exercise)
    {
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }
        
        exercise.UpdatedAt = DateTime.UtcNow;
        return await _exerciseRepository.UpdateAsync(exercise);
    }
    
    /// <summary>
    /// Delete an exercise
    /// </summary>
    public async Task DeleteExerciseAsync(Guid id)
    {
        await _exerciseRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Check if a solution to an exercise is correct
    /// </summary>
    public async Task<bool> CheckSolutionAsync(Guid exerciseId, Guid userId, string submittedQuery)
    {
        try
        {
            var exercise = await _exerciseRepository.GetByIdAsync(exerciseId);
            if (exercise == null)
            {
                throw new ArgumentException($"Exercise with ID {exerciseId} not found");
            }
            
            return await CheckExerciseSolution(exercise, submittedQuery);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error checking solution for exercise {ExerciseId} by user {UserId}", exerciseId, userId);
            return false;
        }
    }

    /// <summary>
    /// Submit a solution for an exercise
    /// </summary>
    public async Task<SolutionResultDto> SubmitSolutionAsync(Guid exerciseId, Guid userId, string submittedQuery)
    {
        try
        {
            var exercise = await _exerciseRepository.GetByIdAsync(exerciseId);
            if (exercise == null)
            {
                throw new ArgumentException($"Exercise with ID {exerciseId} not found");
            }
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found");
            }
            
            // Check solution correctness
            bool isCorrect = await CheckExerciseSolution(exercise, submittedQuery);
            
            // Get current attempt count
            int attemptCount = await _userSolutionRepository.GetAttemptCountAsync(userId, exerciseId);
            
            // Create and save the solution
            var solution = new UserSolution
            {
                UserId = userId,
                ExerciseId = exerciseId,
                SubmittedQuery = submittedQuery,
                IsCorrect = isCorrect,
                AttemptCount = attemptCount + 1
            };
            
            await _userSolutionRepository.AddAsync(solution);
            
            // Update exercise and user statistics
            if (isCorrect && !await _userSolutionRepository.HasUserSolvedExerciseAsync(userId, exerciseId))
            {
                // Update exercise stats
                exercise.SolvedCount++;
                await _exerciseRepository.UpdateAsync(exercise);
                
                // Update user stats
                user.SolvedExercisesCount++;
                await _userRepository.UpdateAsync(user);
            }
            
            // Update attempts count
            exercise.AttemptsCount++;
            await _exerciseRepository.UpdateAsync(exercise);
            
            user.TotalAttemptsCount++;
            await _userRepository.UpdateAsync(user);
            
            // Return the result
            return new SolutionResultDto
            {
                IsCorrect = isCorrect,
                AttemptCount = attemptCount + 1,
                ExerciseId = exerciseId,
                UserId = userId
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error submitting solution for exercise {ExerciseId} by user {UserId}", exerciseId, userId);
            
            return new SolutionResultDto
            {
                IsCorrect = false,
                ExerciseId = exerciseId,
                UserId = userId,
                Feedback = $"Error processing solution: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Get exercise statistics for a user
    /// </summary>
    public async Task<UserExerciseStatsDto> GetUserExerciseStatsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found");
        }
        
        var totalExercises = await _exerciseRepository.GetAllAsync();
        var solvedCount = await _userSolutionRepository.GetCorrectSolutionsByUserAsync(userId);
        
        return new UserExerciseStatsDto
        {
            UserId = userId,
            Username = user.UserName ?? string.Empty,
            TotalExercises = totalExercises.Count,
            SolvedExercises = user.SolvedExercisesCount,
            TotalAttempts = user.TotalAttemptsCount,
            LikedUnits = user.LikedUnitsCount
        };
    }

    /// <summary>
    /// Get a list of exercises solved by a user
    /// </summary>
    public async Task<List<ExerciseDto>> GetSolvedExercisesAsync(Guid userId)
    {
        var solutions = await _userSolutionRepository.GetCorrectSolutionsByUserAsync(userId);
        var exerciseIds = solutions.Select(s => s.ExerciseId).Distinct().ToList();
        
        var exercises = new List<ExerciseDto>();
        
        foreach (var id in exerciseIds)
        {
            var exercise = await _exerciseRepository.GetByIdAsync(id);
            if (exercise != null)
            {
                exercises.Add(MapExerciseToDto(exercise));
            }
        }
        
        return exercises;
    }

    /// <summary>
    /// Get a list of exercises not yet solved by a user
    /// </summary>
    public async Task<List<ExerciseDto>> GetUnsolvedExercisesAsync(Guid userId)
    {
        var solutions = await _userSolutionRepository.GetCorrectSolutionsByUserAsync(userId);
        var solvedIds = solutions.Select(s => s.ExerciseId).Distinct().ToList();
        
        var allExercises = await _exerciseRepository.GetAllAsync();
        var unsolvedExercises = allExercises
            .Where(e => !solvedIds.Contains(e.Id))
            .Select(e => MapExerciseToDto(e))
            .ToList();
        
        return unsolvedExercises;
    }
    
    /// <summary>
    /// Check if a solution is correct based on the exercise type and check method
    /// </summary>
    private async Task<bool> CheckExerciseSolution(Exercise exercise, string submittedQuery)
    {
        // First try string comparison (fastest method)
        if (_sqlExecutionService.CheckByStringComparison(submittedQuery, exercise.SolutionQuery))
        {
            return true;
        }
        
        // If string comparison fails, use the specified check method
        switch (exercise.CheckType)
        {
            case CheckType.Compare:
                // Execute both queries and compare results
                return await _sqlExecutionService.CheckByExecutionAsync(
                    submittedQuery, 
                    exercise.SolutionQuery, 
                    exercise.Schema, 
                    exercise.CheckQueryInsert);
                
            case CheckType.Select:
                // Just execute the user's query and compare with solution output
                return await _sqlExecutionService.CheckByExecutionAsync(
                    submittedQuery, 
                    exercise.CheckQuerySelect, 
                    exercise.Schema);
                
            case CheckType.InsertAndSelect:
                // Execute user's insert query, then check with select
                return await _sqlExecutionService.CheckByExecutionAsync(
                    exercise.CheckQuerySelect, 
                    exercise.CheckQuerySelect, 
                    exercise.Schema, 
                    submittedQuery);
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Map an Exercise entity to ExerciseDto
    /// </summary>
    private ExerciseDto MapExerciseToDto(Exercise exercise)
    {
        return new ExerciseDto
        {
            Id = exercise.Id,
            CreatedAt = exercise.CreatedAt,
            UpdatedAt = exercise.UpdatedAt,
            UnitId = exercise.UnitId,
            UnitTitle = exercise.Unit?.Title ?? string.Empty,
            Title = exercise.Title,
            Description = exercise.Description,
            Difficulty = exercise.Difficulty,
            Type = exercise.Type,
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