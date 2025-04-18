using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

public interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(Guid id);
    Task<List<Unit>> GetAllAsync();
    Task<Unit> CreateAsync(Unit unit);
    Task<Unit> UpdateAsync(Unit unit);
    Task DeleteAsync(Guid id);
}