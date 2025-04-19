using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserSolutionRepository : GenericRepository<UserSolution>, IUserSolutionRepository
{
    private readonly AppDbContext _dbContext;
    
    public UserSolutionRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserSolution?> GetUserSolutionAsync(Guid userId, Guid exerciseId)
    {
        return await _dbContext.UserSolutions
            .Where(s => s.UserId == userId && s.ExerciseId == exerciseId)
            .OrderByDescending(s => s.AttemptCount)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserSolution>> GetSolutionsByExerciseAsync(Guid exerciseId)
    {
        return await _dbContext.UserSolutions
            .Where(s => s.ExerciseId == exerciseId)
            .Include(s => s.User)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<UserSolution>> GetSolutionsByUserAsync(Guid userId)
    {
        return await _dbContext.UserSolutions
            .Where(s => s.UserId == userId)
            .Include(s => s.Exercise)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<UserSolution>> GetCorrectSolutionsByUserAsync(Guid userId)
    {
        return await _dbContext.UserSolutions
            .Where(s => s.UserId == userId && s.IsCorrect)
            .Include(s => s.Exercise)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasUserSolvedExerciseAsync(Guid userId, Guid exerciseId)
    {
        return await _dbContext.UserSolutions
            .AnyAsync(s => s.UserId == userId && s.ExerciseId == exerciseId && s.IsCorrect);
    }

    public async Task<int> GetAttemptCountAsync(Guid userId, Guid exerciseId)
    {
        return await _dbContext.UserSolutions
            .CountAsync(s => s.UserId == userId && s.ExerciseId == exerciseId);
    }
} 