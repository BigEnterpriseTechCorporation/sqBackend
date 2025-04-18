using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly AppDbContext _dbContext;

    public UnitRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> CreateAsync(Unit unit)
    {
        await _dbContext.Units.AddAsync(unit);
        await _dbContext.SaveChangesAsync();
        return unit;
    }

    public async Task<Unit?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Units
            .Include(u => u.Owner)
            .Include(u => u.Exercises)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<List<Unit>> GetAllAsync()
    {
        return await _dbContext.Units
            .Include(u => u.Owner)
            .Include(u => u.Exercises)
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public async Task<Unit> UpdateAsync(Unit unit)
    {
        _dbContext.Units.Update(unit);
        await _dbContext.SaveChangesAsync();
        return unit;
    }

    public async Task DeleteAsync(Guid id)
    {
        var unit = await GetByIdAsync(id);
        if (unit != null)
        {
            unit.IsActive = false;
            unit.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
}