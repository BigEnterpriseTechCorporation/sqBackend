using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of the user service for account management operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    /// <summary>
    /// Constructor for UserService
    /// </summary>
    public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Register a new user in the system
    /// </summary>
    /// <param name="user">The user entity to register</param>
    /// <param name="password">The plaintext password to hash and store</param>
    /// <returns>The created user with generated ID</returns>
    /// <exception cref="InvalidOperationException">Thrown when username already exists</exception>
    /// <exception cref="ArgumentNullException">Thrown when user or password is null</exception>
    public async Task<User> RegisterUserAsync(User user, string password)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");
        
        if (string.IsNullOrWhiteSpace(user.UserName))
            throw new ArgumentException("Username cannot be empty", nameof(user));
            
        try
        {
            if (await _userRepository.UsernameExistsAsync(user.UserName))
                throw new InvalidOperationException($"Username '{user.UserName}' already exists");
            
            // Set default values
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;
            
            // Set default role if not specified
            if (string.IsNullOrWhiteSpace(user.Role))
                user.Role = "Member";
                
            return await _userRepository.CreateAsync(user);
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException && ex is not ArgumentNullException)
        {
            Log.Error(ex, "Error while registering user {Username}", user.UserName);
            throw new InvalidOperationException("Failed to register user. Please try again later.", ex);
        }
    }
    
    /// <summary>
    /// Get a user by their ID
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The user entity if found, null otherwise</returns>
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid user ID", nameof(id));
            
        try
        {
            return await _userRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Get all users in the system
    /// </summary>
    /// <returns>A list of all users</returns>
    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            return await _userRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error retrieving all users");
            throw;
        }
    }

    /// <summary>
    /// Update an existing user's information
    /// </summary>
    /// <param name="user">The user entity with updated values</param>
    /// <returns>A task representing the operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when user is null</exception>
    public async Task UpdateUserAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");
            
        if (user.Id == Guid.Empty)
            throw new ArgumentException("Invalid user ID", nameof(user));
            
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating user with ID {UserId}", user.Id);
            throw;
        }
    }

    /// <summary>
    /// Delete a user from the system (soft delete)
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <returns>A task representing the operation</returns>
    public async Task DeleteUserAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid user ID", nameof(id));
            
        try
        {
            await _userRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }
    
    /// <summary>
    /// Validate a user's credentials
    /// </summary>
    /// <param name="username">The username to validate</param>
    /// <param name="password">The password to check</param>
    /// <returns>True if credentials are valid, false otherwise</returns>
    public async Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        try
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !user.IsActive) 
                return false;

            var result = _passwordHasher.VerifyHashedPassword(
                user, 
                user.PasswordHash, 
                password);

            return result == PasswordVerificationResult.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error validating credentials for user {Username}", username);
            return false;
        }
    }

    /// <summary>
    /// Update a user's profile information with password verification
    /// </summary>
    /// <param name="userId">The ID of the user to update</param>
    /// <param name="currentPassword">The current password for verification</param>
    /// <param name="newUsername">The new username (optional)</param>
    /// <param name="newFullName">The new full name (optional)</param>
    /// <param name="newPassword">The new password (optional)</param>
    /// <returns>The updated user</returns>
    /// <exception cref="ArgumentException">Thrown for invalid arguments</exception>
    /// <exception cref="InvalidOperationException">Thrown when password verification fails or username already exists</exception>
    public async Task<User> UpdateUserProfileAsync(Guid userId, string currentPassword, string? newUsername = null, string? newFullName = null, string? newPassword = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user ID", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(currentPassword))
            throw new ArgumentException("Current password is required", nameof(currentPassword));

        try
        {
            // Get the user
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");
                
            // Verify current password
            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                currentPassword);
                
            if (verificationResult != PasswordVerificationResult.Success)
                throw new InvalidOperationException("Current password is incorrect");

            // Update username if provided and different
            if (!string.IsNullOrWhiteSpace(newUsername) && newUsername != user.UserName)
            {
                // Check if username is already taken
                if (await _userRepository.UsernameExistsAsync(newUsername))
                    throw new InvalidOperationException($"Username '{newUsername}' is already taken");
                    
                user.UserName = newUsername;
                // Update email since it's derived from username
                user.Email = $"{newUsername}@localhost";
            }
            
            // Update full name if provided
            if (!string.IsNullOrWhiteSpace(newFullName) && newFullName != user.FullName)
            {
                user.FullName = newFullName;
            }
            
            // Update password if provided
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            }
            
            // Update the timestamp
            user.UpdatedAt = DateTime.UtcNow;
            
            // Save changes
            await _userRepository.UpdateAsync(user);
            
            // Remove password hash before returning
            var updatedUser = await _userRepository.GetByIdAsync(userId);
            if (updatedUser != null)
                updatedUser.PasswordHash = null;
                
            return updatedUser!;
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException)
        {
            Log.Error(ex, "Error updating profile for user with ID {UserId}", userId);
            throw new InvalidOperationException("Failed to update user profile. Please try again later.", ex);
        }
    }
}