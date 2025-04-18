using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Infrastructure.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUnitRepository _unitRepository;

    public ExerciseService(IExerciseRepository exerciseRepository, IUnitRepository unitRepository)
    {
        _exerciseRepository = exerciseRepository;
        _unitRepository = unitRepository;
    }

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
            
            var result = await _exerciseRepository.CreateAsync(exercise);
            
            if (unit.Exercises == null)
            {
                unit.Exercises = new List<Exercise>();
            }
            unit.Exercises.Add(result);
            unit.UpdatedAt = DateTime.UtcNow;
            await _unitRepository.UpdateAsync(unit);
            
            scope.Complete();
            return result;
        }
        catch
        {
            // Transaction will automatically be rolled back if we don't call Complete()
            throw;
        }
    }

    public async Task<Exercise?> GetExerciseByIdAsync(Guid id)
    {
        return await _exerciseRepository.GetByIdAsync(id);
    }

    public async Task<List<Exercise>> GetAllExercisesAsync()
    {
        return await _exerciseRepository.GetAllAsync();
    }

    public async Task<Exercise> UpdateExerciseAsync(Exercise exercise)
    {
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }
        
        exercise.UpdatedAt = DateTime.UtcNow;
        return await _exerciseRepository.UpdateAsync(exercise);
    }

    public async Task DeleteExerciseAsync(Guid id)
    {
        await _exerciseRepository.DeleteAsync(id);
    }
} 