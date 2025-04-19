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
    
    /// <summary>
    /// Update a user's profile information with password verification
    /// </summary>
    /// <param name="userId">The ID of the user to update</param>
    /// <param name="currentPassword">The current password for verification</param>
    /// <param name="newUsername">The new username (optional)</param>
    /// <param name="newFullName">The new full name (optional)</param>
    /// <param name="newPassword">The new password (optional)</param>
    /// <returns>The updated user</returns>
    Task<User> UpdateUserProfileAsync(Guid userId, string currentPassword, string? newUsername = null, string? newFullName = null, string? newPassword = null);
}