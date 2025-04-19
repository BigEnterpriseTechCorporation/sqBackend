using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs.Requests;

/// <summary>
/// Request model for updating user profile information
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// The new username (optional)
    /// </summary>
    /// <example>johndoe2</example>
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers and underscores")]
    public string? UserName { get; set; }
    
    /// <summary>
    /// The new full name (optional)
    /// </summary>
    /// <example>John D. Doe</example>
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }
    
    /// <summary>
    /// The current password (required for verification)
    /// </summary>
    /// <example>CurrentP@ssw0rd</example>
    [Required(ErrorMessage = "Current password is required for verification")]
    public string CurrentPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// The new password (optional)
    /// </summary>
    /// <example>NewP@ssw0rd123</example>
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string? NewPassword { get; set; }
} 