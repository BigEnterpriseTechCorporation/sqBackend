using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Services;

public interface IExerciseService
{
    Task<Exercise> CreateExerciseAsync(Guid unitId, Exercise exercise);
    Task<Exercise?> GetExerciseByIdAsync(Guid id);
    Task<List<Exercise>> GetAllExercisesAsync();
    Task<Exercise> UpdateExerciseAsync(Exercise exercise);
    Task DeleteExerciseAsync(Guid id);
} 