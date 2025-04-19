namespace ApplicationCore.Entities;

/// <summary>
/// Types of checking methods for exercise solutions
/// </summary>
public enum CheckType
{
    /// <summary>
    /// Compare query outputs
    /// </summary>
    Compare,
    
    /// <summary>
    /// Run select query only
    /// </summary>
    Select,
    
    /// <summary>
    /// Run insert and then select query
    /// </summary>
    InsertAndSelect,
}

/// <summary>
/// Difficulty levels for exercises
/// </summary>
public enum Difficulty
{
    /// <summary>
    /// Easy difficulty
    /// </summary>
    Easy,
    
    /// <summary>
    /// Normal difficulty
    /// </summary>
    Normal,
    
    /// <summary>
    /// Hard difficulty
    /// </summary>
    Hard,
    
    /// <summary>
    /// Ultra hard difficulty
    /// </summary>
    UltraHard
}

/// <summary>
/// Types of SQL exercises
/// </summary>
public enum ExerciseType
{
    /// <summary>
    /// Select from multiple-choice options
    /// </summary>
    SelectAnswer,
    
    /// <summary>
    /// Fill in missing parts of SQL/code
    /// </summary>
    FillMissingWords,
    
    /// <summary>
    /// Build a query from given parts/words
    /// </summary>
    ConstructQuery,
    
    /// <summary>
    /// Write a simple SQL query from scratch
    /// </summary>
    SimpleQuery,
    
    /// <summary>
    /// Write a complex SQL query from scratch
    /// </summary>
    ComplexQuery
}

/// <summary>
/// Entity representing a SQL exercise
/// </summary>
public class Exercise : BaseEntity
{
    /// <summary>
    /// Title of the exercise
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the exercise
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the exercise is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// ID of the unit this exercise belongs to
    /// </summary>
    public Guid UnitId { get; set; }
    
    /// <summary>
    /// Navigation property to the unit
    /// </summary>
    public Unit? Unit { get; set; }

    /// <summary>
    /// Difficulty level of the exercise
    /// </summary>
    public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    
    /// <summary>
    /// Type of the exercise
    /// </summary>
    public ExerciseType Type { get; set; } = ExerciseType.SimpleQuery;
    
    /// <summary>
    /// SQL schema definition for the exercise
    /// </summary>
    public string Schema { get; set; } = string.Empty;
    
    /// <summary>
    /// Method used to check solutions
    /// </summary>
    public CheckType CheckType { get; set; } = CheckType.Compare;
    
    /// <summary>
    /// Query to insert test data
    /// </summary>
    public string CheckQueryInsert { get; set; } = string.Empty;
    
    /// <summary>
    /// Query to select test results
    /// </summary>
    public string CheckQuerySelect { get; set; } = string.Empty;
    
    /// <summary>
    /// The correct solution query
    /// </summary>
    public string SolutionQuery { get; set; } = string.Empty;
    
    /// <summary>
    /// Hash of the solution output (for optimization)
    /// </summary>
    public string SolutionOutputHash { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON array of options for SelectAnswer and FillMissingWords types
    /// </summary>
    public string Options { get; set; } = string.Empty;
    
    /// <summary>
    /// JSON array of query parts for ConstructQuery type
    /// </summary>
    public string QueryParts { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of times this exercise has been solved
    /// </summary>
    public int SolvedCount { get; set; }
    
    /// <summary>
    /// Number of attempts made on this exercise
    /// </summary>
    public int AttemptsCount { get; set; }
    
    /// <summary>
    /// Navigation property to user solutions for this exercise
    /// </summary>
    public List<UserSolution>? UserSolutions { get; set; } = [];
}