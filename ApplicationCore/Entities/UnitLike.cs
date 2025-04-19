using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

/// <summary>
/// Represents a like given to a unit by a user
/// </summary>
public class UnitLike : BaseEntity
{
    /// <summary>
    /// The ID of the user who liked the unit
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// The ID of the unit that was liked
    /// </summary>
    public Guid UnitId { get; set; }
    
    /// <summary>
    /// The date and time when the unit was liked
    /// </summary>
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property for the user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
    
    /// <summary>
    /// Navigation property for the unit
    /// </summary>
    [ForeignKey(nameof(UnitId))]
    public virtual Unit? Unit { get; set; }
} 