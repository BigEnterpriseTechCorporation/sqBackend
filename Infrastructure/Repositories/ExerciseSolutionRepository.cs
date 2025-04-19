using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExerciseSolutionRepository : GenericRepository<ExerciseSolution>, IExerciseSolutionRepository
{
    private readonly AppDbContext _dbContext;
    
    public ExerciseSolutionRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExerciseSolution?> GetUserSolutionAsync(Guid userId, Guid exerciseId)
    {
        return await _dbContext.ExerciseSolutions
            .Where(s => s.UserId == userId && s.ExerciseId == exerciseId)
            .OrderByDescending(s => s.SubmittedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ExerciseSolution>> GetSolutionsByExerciseAsync(Guid exerciseId)
    {
        return await _dbContext.ExerciseSolutions
            .Where(s => s.ExerciseId == exerciseId)
            .Include(s => s.User)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<ExerciseSolution>> GetSolutionsByUserAsync(Guid userId)
    {
        return await _dbContext.ExerciseSolutions
            .Where(s => s.UserId == userId)
            .Include(s => s.Exercise)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<ExerciseSolution>> GetCorrectSolutionsByUserAsync(Guid userId)
    {
        return await _dbContext.ExerciseSolutions
            .Where(s => s.UserId == userId && s.IsCorrect)
            .Include(s => s.Exercise)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<bool> HasUserSolvedExerciseAsync(Guid userId, Guid exerciseId)
    {
        return await _dbContext.ExerciseSolutions
            .AnyAsync(s => s.UserId == userId && s.ExerciseId == exerciseId && s.IsCorrect);
    }

    public async Task<int> GetAttemptCountAsync(Guid userId, Guid exerciseId)
    {
        return await _dbContext.ExerciseSolutions
            .CountAsync(s => s.UserId == userId && s.ExerciseId == exerciseId);
    }
} 