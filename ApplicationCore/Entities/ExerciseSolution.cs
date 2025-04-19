using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities;

/// <summary>
/// Represents a solution submitted by a user for an exercise
/// </summary>
public class ExerciseSolution : BaseEntity
{
    /// <summary>
    /// The ID of the user who submitted the solution
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// The ID of the exercise for which the solution was submitted
    /// </summary>
    public Guid ExerciseId { get; set; }
    
    /// <summary>
    /// The submitted solution text/query
    /// </summary>
    public string Solution { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the solution is correct
    /// </summary>
    public bool IsCorrect { get; set; }
    
    /// <summary>
    /// The date and time when the solution was submitted
    /// </summary>
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Navigation property for the user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
    
    /// <summary>
    /// Navigation property for the exercise
    /// </summary>
    [ForeignKey(nameof(ExerciseId))]
    public virtual Exercise? Exercise { get; set; }
} 