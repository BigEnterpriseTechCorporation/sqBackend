using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

public interface IExerciseRepository
{
    Task<Exercise> CreateAsync(Exercise exercise);
    Task<Exercise?> GetByIdAsync(Guid id);
    Task<List<Exercise>> GetAllAsync();
    Task<Exercise> UpdateAsync(Exercise exercise);
    Task DeleteAsync(Guid id);
} 