namespace ApplicationCore.DTOs;

/// <summary>
/// DTO for exercise solution submission results
/// </summary>
public class SolutionResultDto
{
    /// <summary>
    /// Whether the solution is correct
    /// </summary>
    public bool IsCorrect { get; set; }
    
    /// <summary>
    /// Number of attempts made by the user on this exercise
    /// </summary>
    public int AttemptCount { get; set; }
    
    /// <summary>
    /// Optional feedback message (e.g., error details if execution failed)
    /// </summary>
    public string? Feedback { get; set; }
    
    /// <summary>
    /// The exercise ID
    /// </summary>
    public Guid ExerciseId { get; set; }
    
    /// <summary>
    /// The user ID
    /// </summary>
    public Guid UserId { get; set; }
} 