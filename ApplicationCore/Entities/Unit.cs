namespace ApplicationCore.Entities;

/// <summary>
/// Entity representing a collection of exercises
/// </summary>
public class Unit : BaseEntity
{
    /// <summary>
    /// Title of the unit
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the unit
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the unit is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Number of likes this unit has received
    /// </summary>
    public int LikesCount { get; set; }
    
    /// <summary>
    /// ID of the user who owns the unit
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Navigation property to the owner
    /// </summary>
    public User? Owner { get; set; }
    
    /// <summary>
    /// Navigation property to the exercises in this unit
    /// </summary>
    public List<Exercise>? Exercises { get; set; } = [];
    
    /// <summary>
    /// Navigation property to the likes on this unit
    /// </summary>
    public List<UnitLike>? Likes { get; set; } = [];
}