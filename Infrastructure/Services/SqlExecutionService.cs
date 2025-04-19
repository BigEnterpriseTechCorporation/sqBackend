using System.Data;
using ApplicationCore.Interfaces.Services;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Infrastructure.Services;

/// <summary>
/// Service that executes SQL queries in a safe, isolated in-memory SQLite environment
/// </summary>
public class SqlExecutionService : ISqlExecutionService
{
    /// <summary>
    /// Check if a solution is correct by comparing strings (case-insensitive)
    /// </summary>
    public bool CheckByStringComparison(string userQuery, string solutionQuery)
    {
        if (string.IsNullOrWhiteSpace(userQuery) || string.IsNullOrWhiteSpace(solutionQuery))
        {
            return false;
        }
        
        // Normalize queries by trimming whitespace and making lowercase
        string normalizedUserQuery = NormalizeQuery(userQuery);
        string normalizedSolutionQuery = NormalizeQuery(solutionQuery);
        
        return normalizedUserQuery == normalizedSolutionQuery;
    }
    
    /// <summary>
    /// Check if a solution is correct by executing both queries and comparing results
    /// </summary>
    public async Task<bool> CheckByExecutionAsync(string userQuery, string solutionQuery, string schemaDefinition, string insertQuery = "")
    {
        try
        {
            // Execute both queries and get results
            var userResults = await ExecuteQueryAsync(userQuery, schemaDefinition, insertQuery);
            var solutionResults = await ExecuteQueryAsync(solutionQuery, schemaDefinition, insertQuery);
            
            // Compare results
            return CompareDataTables(userResults, solutionResults);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing and comparing SQL queries");
            return false;
        }
    }
    
    /// <summary>
    /// Execute a query and return the results as a DataTable
    /// </summary>
    public async Task<DataTable> ExecuteQueryAsync(string query, string schemaDefinition, string insertQuery = "")
    {
        var dataTable = new DataTable();
        
        try
        {
            // Create a new in-memory SQLite connection
            using var connection = CreateInMemoryConnection();
            await connection.OpenAsync();
            
            // Create schema
            if (!string.IsNullOrWhiteSpace(schemaDefinition))
            {
                using var schemaCommand = connection.CreateCommand();
                schemaCommand.CommandText = schemaDefinition;
                await schemaCommand.ExecuteNonQueryAsync();
            }
            
            // Insert test data if provided
            if (!string.IsNullOrWhiteSpace(insertQuery))
            {
                using var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = insertQuery;
                await insertCommand.ExecuteNonQueryAsync();
            }
            
            // Execute the query
            using var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandTimeout = 5; // 5 second timeout for safety
            
            // Read the results into a DataTable
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);
            
            return dataTable;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error executing SQL query: {Query}", query);
            throw;
        }
    }
    
    /// <summary>
    /// Creates an in-memory SQLite connection
    /// </summary>
    private static SqliteConnection CreateInMemoryConnection()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = ":memory:",
            Mode = SqliteOpenMode.Memory,
            Cache = SqliteCacheMode.Shared
        };
        
        return new SqliteConnection(connectionStringBuilder.ConnectionString);
    }
    
    /// <summary>
    /// Normalizes a SQL query by removing extra whitespace, comments and making it lowercase
    /// </summary>
    private static string NormalizeQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return string.Empty;
        }
        
        // Remove comments
        var lines = query.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => !line.TrimStart().StartsWith("--"))
            .ToArray();
        
        // Join lines, normalize whitespace, and make lowercase
        var normalizedQuery = string.Join(" ", lines)
            .Replace("\t", " ")
            .Replace(";", "")
            .ToLowerInvariant();
        
        // Replace multiple spaces with a single space
        while (normalizedQuery.Contains("  "))
        {
            normalizedQuery = normalizedQuery.Replace("  ", " ");
        }
        
        return normalizedQuery.Trim();
    }
    
    /// <summary>
    /// Compares two DataTables for equality
    /// </summary>
    private static bool CompareDataTables(DataTable table1, DataTable table2)
    {
        // Check if both tables have the same number of rows
        if (table1.Rows.Count != table2.Rows.Count)
        {
            return false;
        }
        
        // Check if both tables have the same columns
        if (table1.Columns.Count != table2.Columns.Count)
        {
            return false;
        }
        
        // Compare data row by row, column by column
        for (int i = 0; i < table1.Rows.Count; i++)
        {
            for (int j = 0; j < table1.Columns.Count; j++)
            {
                var value1 = table1.Rows[i][j];
                var value2 = table2.Rows[i][j];
                
                // Handle DBNull
                if (value1 is DBNull && value2 is DBNull)
                {
                    continue;
                }
                
                if ((value1 is DBNull && value2 is not DBNull) || 
                    (value1 is not DBNull && value2 is DBNull))
                {
                    return false;
                }
                
                // Compare values
                if (!value1.Equals(value2))
                {
                    return false;
                }
            }
        }
        
        return true;
    }
} 