using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Services;

public interface IUserService
{
    Task<User> RegisterUserAsync(User user, string password);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<List<User>> GetAllUsersAsync();
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
    Task<bool> ValidateCredentialsAsync(string email, string password);
}