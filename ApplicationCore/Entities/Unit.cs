namespace ApplicationCore.Entities;

public class Unit
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // create time
    public DateTime? UpdatedAt { get; set; } // last time of update
    public bool IsActive { get; set; } = true; // soft deletion marker
    
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public List<Exercise> Exercises { get; set; } = [];
}