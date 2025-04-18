using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Infrastructure.Services;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;
    
    public UnitService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }
    
    public async Task<Unit> CreateUnitAsync(User creator, string title, string description)
    {
        if (creator == null)
        {
            throw new ArgumentNullException(nameof(creator));
        }
        
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }
        
        Unit unit = new Unit
        {
            Title = title,
            Description = description ?? string.Empty,
            Owner = creator,
            OwnerId = creator.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Exercises = new List<Exercise>()
        };
        
        return await _unitRepository.CreateAsync(unit);
    }

    public async Task<Unit?> GetUnitByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Invalid unit ID", nameof(id));
        }
        
        return await _unitRepository.GetByIdAsync(id);
    }

    public async Task<List<Unit>> GetAllUnitsAsync()
    {
        return await _unitRepository.GetAllAsync();
    }

    public async Task<List<Exercise>> GetAllExercisesAsync(Guid unitId)
    {
        if (unitId == Guid.Empty)
        {
            throw new ArgumentException("Invalid unit ID", nameof(unitId));
        }
        
        var unit = await GetUnitByIdAsync(unitId);
        
        if (unit == null)
        {
            throw new ArgumentException($"Unit with ID {unitId} not found");
        }
        
        return unit.Exercises?.Where(e => e.IsActive).ToList() ?? new List<Exercise>();
    }

    public async Task AddExerciseAsync(Guid unitId, Exercise exercise)
    {
        if (unitId == Guid.Empty)
        {
            throw new ArgumentException("Invalid unit ID", nameof(unitId));
        }
        
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }
        
        var unit = await GetUnitByIdAsync(unitId);
        if (unit != null)
        {
            if (unit.Exercises == null)
            {
                unit.Exercises = new List<Exercise>();
            }
            
            exercise.UnitId = unitId;
            unit.Exercises.Add(exercise);
            
            unit.UpdatedAt = DateTime.UtcNow;
            await _unitRepository.UpdateAsync(unit);
        }
        else
        {
            throw new ArgumentException($"Unit with ID {unitId} not found");
        }
    }

    public async Task UpdateUnitAsync(Unit unit)
    {
        if (unit == null)
        {
            throw new ArgumentNullException(nameof(unit));
        }
        
        unit.UpdatedAt = DateTime.UtcNow;
        await _unitRepository.UpdateAsync(unit);
    }

    public async Task DeleteUnitAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Invalid unit ID", nameof(id));
        }
        
        await _unitRepository.DeleteAsync(id);
    }
}