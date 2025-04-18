using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly AppDbContext _dbContext;

    public ExerciseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Exercise> CreateAsync(Exercise exercise)
    {
        await _dbContext.Exercises.AddAsync(exercise);
        await _dbContext.SaveChangesAsync();
        return exercise;
    }

    public async Task<Exercise?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Exercises
            .Include(e => e.Unit)
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);
    }

    public async Task<List<Exercise>> GetAllAsync()
    {
        return await _dbContext.Exercises
            .Include(e => e.Unit)
            .Where(e => e.IsActive)
            .ToListAsync();
    }

    public async Task<Exercise> UpdateAsync(Exercise exercise)
    {
        _dbContext.Exercises.Update(exercise);
        await _dbContext.SaveChangesAsync();
        return exercise;
    }

    public async Task DeleteAsync(Guid id)
    {
        var exercise = await GetByIdAsync(id);
        if (exercise != null)
        {
            exercise.IsActive = false;
            exercise.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
} 