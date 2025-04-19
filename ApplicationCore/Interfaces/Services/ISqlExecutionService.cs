using System.Data;

namespace ApplicationCore.Interfaces.Services;

/// <summary>
/// Service for executing SQL queries in a safe, isolated environment
/// </summary>
public interface ISqlExecutionService
{
    /// <summary>
    /// Check if a solution is correct by comparing strings (case-insensitive)
    /// </summary>
    /// <param name="userQuery">The user's submitted query</param>
    /// <param name="solutionQuery">The correct solution query</param>
    /// <returns>True if the solution is correct</returns>
    bool CheckByStringComparison(string userQuery, string solutionQuery);
    
    /// <summary>
    /// Check if a solution is correct by executing both queries and comparing results
    /// </summary>
    /// <param name="userQuery">The user's submitted query</param>
    /// <param name="solutionQuery">The correct solution query</param>
    /// <param name="schemaDefinition">The schema to create for the test database</param>
    /// <param name="insertQuery">Optional query to insert test data</param>
    /// <returns>True if the solution is correct, false otherwise</returns>
    Task<bool> CheckByExecutionAsync(string userQuery, string solutionQuery, string schemaDefinition, string insertQuery = "");
    
    /// <summary>
    /// Execute a query and return the results as a DataTable
    /// </summary>
    /// <param name="query">The SQL query to execute</param>
    /// <param name="schemaDefinition">The schema to create for the test database</param>
    /// <param name="insertQuery">Optional query to insert test data</param>
    /// <returns>A DataTable with the query results</returns>
    Task<DataTable> ExecuteQueryAsync(string query, string schemaDefinition, string insertQuery = "");
} 