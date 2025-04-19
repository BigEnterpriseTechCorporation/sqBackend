using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs.Requests;

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The username for the account
    /// </summary>
    /// <example>johndoe</example>
    [Required(ErrorMessage = "Username is required")]
    public string username { get; set; } = string.Empty;
    
    /// <summary>
    /// The password for the account
    /// </summary>
    /// <example>SecureP@ssw0rd</example>
    [Required(ErrorMessage = "Password is required")]
    public string password { get; set; } = string.Empty;
}