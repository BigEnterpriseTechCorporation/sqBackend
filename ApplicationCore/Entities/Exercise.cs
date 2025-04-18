namespace ApplicationCore.Entities;

public enum CheckType
{
    Compare, // compare outputs
    Select, // run select only
    InsertAndSelect, // run insert and select
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    UltraHard
}

public enum ExerciseType
{
    SelectAnswer,       // Select from multiple-choice options
    FillMissingWords,   // Fill in missing parts of SQL/code
    ConstructQuery,     // Build a query from given parts/words
    SimpleQuery,        // Write a simple SQL query from scratch
    ComplexQuery        // Write a complex SQL query from scratch
}

public class Exercise
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // create time
    public DateTime? UpdatedAt { get; set; } // last time of update
    public bool IsActive { get; set; } = true; // soft deletion marker
    
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; }

    public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    public ExerciseType Type { get; set; } = ExerciseType.SimpleQuery;
    
    public string Schema { get; set; } = string.Empty;
    public CheckType CheckType { get; set; } = CheckType.Compare;
    
    public string CheckQueryInsert { get; set; } = string.Empty;
    public string CheckQuerySelect { get; set; } = string.Empty;
    
    public string SolutionQuery { get; set; } = string.Empty;
    public string SolutionOutputHash { get; set; } = string.Empty;
    
    // For SelectAnswer and FillMissingWords types
    public string Options { get; set; } = string.Empty; // JSON array of options
    
    // For ConstructQuery type
    public string QueryParts { get; set; } = string.Empty; // JSON array of query parts that can be used
}