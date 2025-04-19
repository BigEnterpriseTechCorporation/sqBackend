using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs.Requests;

/// <summary>
/// Request model for updating a user's active status
/// </summary>
public class UpdateActiveStatusRequest
{
    /// <summary>
    /// Whether the user should be active
    /// </summary>
    /// <example>true</example>
    [Required(ErrorMessage = "Active status is required")]
    public bool IsActive { get; set; }
} 