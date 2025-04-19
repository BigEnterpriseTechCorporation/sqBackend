using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs.Requests;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// The username for the new account
    /// </summary>
    /// <example>johndoe</example>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers and underscores")]
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// The password for the new account
    /// </summary>
    /// <example>SecureP@ssw0rd</example>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// The full name of the user
    /// </summary>
    /// <example>John Doe</example>
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string FullName { get; set; } = string.Empty;
}