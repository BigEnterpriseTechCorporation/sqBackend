using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(Guid id);
    Task<List<Unit>> GetAllAsync();
    Task<Unit> CreateAsync(Unit unit);
    Task UpdateAsync(Unit unit);
    Task DeleteAsync(Guid id);
}