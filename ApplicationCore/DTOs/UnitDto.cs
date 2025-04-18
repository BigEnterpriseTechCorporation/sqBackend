using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.DTOs;

public class UnitDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int ExerciseCount { get; set; }
}

public class UnitDetailDto : UnitDto
{
    public List<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();
}

public class CreateUnitRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;
}

public class UpdateUnitRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;
} 