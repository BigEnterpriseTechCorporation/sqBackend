using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<User> RegisterUserAsync(User user, string password)
    {
        if (await _userRepository.UsernameExistsAsync(user.UserName))
            throw new InvalidOperationException("Username already exists");
        
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        return await _userRepository.CreateAsync(user);
    }

    
    
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }
    
    

    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return false;

        var result = _passwordHasher.VerifyHashedPassword(
            user, 
            user.PasswordHash, 
            password);

        return result == PasswordVerificationResult.Success;
    }
}