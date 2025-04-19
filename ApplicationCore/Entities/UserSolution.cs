namespace ApplicationCore.Entities;

/// <summary>
/// Entity representing a user's solution to an exercise
/// </summary>
public class UserSolution : BaseEntity
{
    /// <summary>
    /// ID of the user who submitted the solution
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// ID of the exercise
    /// </summary>
    public Guid ExerciseId { get; set; }
    
    /// <summary>
    /// The submitted SQL query solution
    /// </summary>
    public string SubmittedQuery { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the solution is correct
    /// </summary>
    public bool IsCorrect { get; set; }
    
    /// <summary>
    /// The number of attempts made before this solution
    /// </summary>
    public int AttemptCount { get; set; }
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User? User { get; set; }
    
    /// <summary>
    /// Navigation property to the exercise
    /// </summary>
    public Exercise? Exercise { get; set; }
} 