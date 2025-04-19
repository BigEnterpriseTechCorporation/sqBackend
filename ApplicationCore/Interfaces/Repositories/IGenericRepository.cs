using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Repositories;

/// <summary>
/// Generic repository interface defining basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Get an entity by its ID
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Get all entities of type T
    /// </summary>
    /// <returns>List of all entities</returns>
    Task<List<T>> GetAllAsync();
    
    /// <summary>
    /// Add a new entity to the database
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>The added entity</returns>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Update an existing entity in the database
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <returns>The updated entity</returns>
    Task<T> UpdateAsync(T entity);
    
    /// <summary>
    /// Delete an entity from the database
    /// </summary>
    /// <param name="id">The ID of the entity to delete</param>
    Task DeleteAsync(Guid id);
} 