using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Services;

public interface IUnitService
{
    Task<Unit> CreateUnitAsync(User creator, string title, string description);
    Task<Unit?> GetUnitByIdAsync(Guid id);
    Task<List<Unit>> GetAllUnitsAsync();
    Task<List<Exercise>> GetAllExercisesAsync(Guid unitId);
    Task AddExerciseAsync(Guid unitId, Exercise exercise);
    Task UpdateUnitAsync(Unit unit);
    Task DeleteUnitAsync(Guid id);
}