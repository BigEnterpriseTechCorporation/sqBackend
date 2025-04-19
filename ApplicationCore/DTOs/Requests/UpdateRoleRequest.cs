using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs.Requests;

/// <summary>
/// Request model for updating a user's role
/// </summary>
public class UpdateRoleRequest
{
    /// <summary>
    /// The new role to assign to the user
    /// </summary>
    /// <example>Admin</example>
    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } = string.Empty;
} 