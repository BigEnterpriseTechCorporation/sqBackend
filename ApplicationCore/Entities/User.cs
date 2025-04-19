using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities;

/// <summary>
/// Entity representing a user of the system
/// </summary>
public class User : IdentityUser<Guid>
{
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Full name of the user
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Username for login (Latin letters only)
    /// </summary>
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username must contain only Latin letters, numbers, and underscores.")]
    public override string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// User role (Member, Admin)
    /// </summary>
    public string Role { get; set; } = "Member";
    
    /// <summary>
    /// Total number of exercises solved by the user
    /// </summary>
    public int SolvedExercisesCount { get; set; }
    
    /// <summary>
    /// Total number of attempts made by the user
    /// </summary>
    public int TotalAttemptsCount { get; set; }
    
    /// <summary>
    /// Total number of units this user has liked
    /// </summary>
    public int LikedUnitsCount { get; set; }
    
    /// <summary>
    /// Navigation property to the units created by this user
    /// </summary>
    public List<Unit> Units { get; set; } = new List<Unit>();
    
    /// <summary>
    /// Navigation property to the solutions submitted by this user
    /// </summary>
    public List<UserSolution> Solutions { get; set; } = new List<UserSolution>();
    
    /// <summary>
    /// Navigation property to the unit likes by this user
    /// </summary>
    public List<UnitLike> UnitLikes { get; set; } = new List<UnitLike>();
}