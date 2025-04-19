using ApplicationCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid UnitId { get; set; }
    public string UnitTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public ExerciseType Type { get; set; }
    public string Schema { get; set; } = string.Empty;
    public CheckType CheckType { get; set; }
    public string CheckQueryInsert { get; set; } = string.Empty;
    public string CheckQuerySelect { get; set; } = string.Empty;
    public string SolutionQuery { get; set; } = string.Empty;
    public string Options { get; set; } = string.Empty;
    public string QueryParts { get; set; } = string.Empty;
}

public class CreateExerciseRequest
{
    [Required]
    public Guid UnitId { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;
    
    public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    
    [Required]
    public ExerciseType Type { get; set; } = ExerciseType.SimpleQuery;
    
    [Required]
    [MinLength(1, ErrorMessage = "Schema cannot be empty")]
    public string Schema { get; set; } = string.Empty;
    
    public CheckType CheckType { get; set; } = CheckType.Compare;
    
    public string CheckQueryInsert { get; set; } = string.Empty;
    
    public string CheckQuerySelect { get; set; } = string.Empty;
    
    public string SolutionQuery { get; set; } = string.Empty;
    
    // For SelectAnswer and FillMissingWords types
    public string Options { get; set; } = string.Empty;
    
    // For ConstructQuery type
    public string QueryParts { get; set; } = string.Empty;
}

public class UpdateExerciseRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;
    
    public Difficulty Difficulty { get; set; }
    
    [Required]
    public ExerciseType Type { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "Schema cannot be empty")]
    public string Schema { get; set; } = string.Empty;
    
    public CheckType CheckType { get; set; }
    
    public string CheckQueryInsert { get; set; } = string.Empty;
    
    public string CheckQuerySelect { get; set; } = string.Empty;
    
    public string SolutionQuery { get; set; } = string.Empty;
    
    // For SelectAnswer and FillMissingWords types
    public string Options { get; set; } = string.Empty;
    
    // For ConstructQuery type
    public string QueryParts { get; set; } = string.Empty;
} 