using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UnitLikeRepository : GenericRepository<UnitLike>, IUnitLikeRepository
{
    private readonly AppDbContext _dbContext;
    
    public UnitLikeRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasUserLikedUnitAsync(Guid userId, Guid unitId)
    {
        return await _dbContext.UnitLikes
            .AnyAsync(l => l.UserId == userId && l.UnitId == unitId);
    }

    public async Task<UnitLike?> GetByUserAndUnitAsync(Guid userId, Guid unitId)
    {
        return await _dbContext.UnitLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.UnitId == unitId);
    }

    public async Task<List<UnitLike>> GetByUserAsync(Guid userId)
    {
        return await _dbContext.UnitLikes
            .Where(l => l.UserId == userId)
            .Include(l => l.Unit)
            .OrderByDescending(l => l.LikedAt)
            .ToListAsync();
    }

    public async Task<List<UnitLike>> GetByUnitAsync(Guid unitId)
    {
        return await _dbContext.UnitLikes
            .Where(l => l.UnitId == unitId)
            .Include(l => l.User)
            .OrderByDescending(l => l.LikedAt)
            .ToListAsync();
    }

    public async Task<int> GetLikesCountAsync(Guid unitId)
    {
        return await _dbContext.UnitLikes
            .CountAsync(l => l.UnitId == unitId);
    }
} 