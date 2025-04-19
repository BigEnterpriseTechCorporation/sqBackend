namespace ApplicationCore.DTOs.Responses;

/// <summary>
/// Response model for system statistics
/// </summary>
public class SystemStatsResponse
{
    /// <summary>
    /// Total number of users in the system
    /// </summary>
    public int TotalUsers { get; set; }
    
    /// <summary>
    /// Number of active users
    /// </summary>
    public int ActiveUsers { get; set; }
    
    /// <summary>
    /// Number of admin users
    /// </summary>
    public int AdminUsers { get; set; }
    
    /// <summary>
    /// User registration statistics by date
    /// </summary>
    public List<DateCountPair> UsersByCreationDate { get; set; } = new List<DateCountPair>();
    
    /// <summary>
    /// Total number of units in the system
    /// </summary>
    public int TotalUnits { get; set; }
    
    /// <summary>
    /// Total number of exercises in the system
    /// </summary>
    public int TotalExercises { get; set; }
    
    /// <summary>
    /// Exercise statistics by type
    /// </summary>
    public Dictionary<string, int> ExercisesByType { get; set; } = new Dictionary<string, int>();
    
    /// <summary>
    /// Exercise statistics by difficulty
    /// </summary>
    public Dictionary<string, int> ExercisesByDifficulty { get; set; } = new Dictionary<string, int>();
}

/// <summary>
/// Helper class for date-based statistics
/// </summary>
public class DateCountPair
{
    /// <summary>
    /// The date
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// The count for that date
    /// </summary>
    public int Count { get; set; }
} 