namespace ApplicationCore.DTOs.Responses;

/// <summary>
/// Response model for paginated data
/// </summary>
/// <typeparam name="T">The type of items in the collection</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// The collection of items for the current page
    /// </summary>
    public List<T> Items { get; set; } = new List<T>();
    
    /// <summary>
    /// The current page number (1-based)
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// The total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// The total number of pages
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;
    
    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
} 