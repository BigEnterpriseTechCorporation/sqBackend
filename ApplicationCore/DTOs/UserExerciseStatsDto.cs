namespace ApplicationCore.DTOs;

/// <summary>
/// DTO for user exercise statistics
/// </summary>
public class UserExerciseStatsDto
{
    /// <summary>
    /// The user ID
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User's username
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// Total number of exercises available
    /// </summary>
    public int TotalExercises { get; set; }
    
    /// <summary>
    /// Number of exercises solved by the user
    /// </summary>
    public int SolvedExercises { get; set; }
    
    /// <summary>
    /// Total number of attempts made by the user across all exercises
    /// </summary>
    public int TotalAttempts { get; set; }
    
    /// <summary>
    /// Number of units liked by the user
    /// </summary>
    public int LikedUnits { get; set; }
    
    /// <summary>
    /// Completion percentage (SolvedExercises / TotalExercises * 100)
    /// </summary>
    public double CompletionPercentage => TotalExercises > 0 
        ? Math.Round((double)SolvedExercises / TotalExercises * 100, 2) 
        : 0;
} 