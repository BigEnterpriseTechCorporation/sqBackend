using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // create time
    public DateTime? UpdatedAt { get; set; } // last time of update
    public bool IsActive { get; set; } = true; // soft deletion marker
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Логин должен содержать только латинские буквы.")]
    public override string UserName { get; set; } = string.Empty;
    
    public string Role { get; set; } = "Member";
}