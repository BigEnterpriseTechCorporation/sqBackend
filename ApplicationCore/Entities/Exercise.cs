namespace ApplicationCore.Entities;

public enum CheckType
{
    Compare, // compare outputs
    Select, // run select only
    InsertAndSelect, // run insert and select
}

public enum Dificulty
{
    Easy,
    Normal,
    Hard,
    UltraHard
}

public class Exercise
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // create time
    public DateTime? UpdatedAt { get; set; } // last time of update
    public bool IsActive { get; set; } = true; // soft deletion marker
    
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; }

    public Dificulty Dificulty { get; set; } = Dificulty.Normal;
    
    public string Schema { get; set; } = string.Empty;
    public CheckType CheckType { get; set; } = CheckType.Compare;
    
    public string CheckQueryInsert { get; set; } = string.Empty;
    public string CheckQuerySelect { get; set; } = string.Empty;
    
    public string SolutionQuery { get; set; } = string.Empty;
    public string SolutionOutputHash { get; set; } = string.Empty;
}