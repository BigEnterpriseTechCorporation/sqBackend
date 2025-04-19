# SQL Solution Checking: Technical Guide

This document provides technical details on how the SQL Learning Platform validates user-submitted SQL queries against expected solutions.

## Related Documentation

- [Exercise Solutions API](exercise_solutions_api.md): API for submitting and managing solutions
- [Developer Guide](developer_guide.md): Architecture and development workflow
- [API Overview](api_overview.md): Summary of all API endpoints

## Overview

The SQL solution checking system is designed to:

1. Safely execute user-submitted SQL queries
2. Compare results against expected outputs
3. Provide meaningful feedback on incorrect solutions
4. Support different types of validation methods
5. Make reference solutions available to users after multiple attempts

## Solution Checking Architecture

The solution validation process follows these steps:

```
User SQL Query → Validation → Execution → Results Comparison → Feedback
```

### Components

1. **Query Sanitizer**: Filters unsafe SQL operations
2. **Query Executor**: Runs queries in an isolated database environment
3. **Results Comparator**: Compares actual results with expected results
4. **Feedback Generator**: Provides hints and error messages

## Reference Solution Management

Each exercise in the system has a reference solution stored in the `SolutionQuery` field. This SQL query represents the correct solution to the exercise and is used in the solution checking process.

### Solution Storage

The reference solution is stored in the Exercise entity and is:
- Set when creating or updating an exercise
- Accessible via the API for authorized users
- Included in the ExerciseDto for client applications to display after multiple failed attempts

### Solution Query Visibility

The `SolutionQuery` field is included in the ExerciseDto returned by the API, which allows client applications to:

1. Hide the solution until a user has attempted the exercise multiple times
2. Show the solution when a user chooses to see it after failed attempts
3. Display the solution after the exercise is completed for learning purposes

Frontend applications should implement appropriate logic to control the visibility of the solution based on the user's progress and attempt count.

## Validation Methods

The platform supports multiple validation methods, defined by the `CheckType` enum:

### 1. String Comparison (Initial Check)

Before applying more complex validation methods, the system first performs a simple string comparison between the user's query and the reference solution:

```csharp
// First try string comparison (fastest method)
if (_sqlExecutionService.CheckByStringComparison(submittedQuery, exercise.SolutionQuery))
{
    return true;
}
```

This string comparison is case-insensitive and normalizes both queries by:
- Removing comments and extra whitespace
- Converting to lowercase
- Removing trailing semicolons
- Normalizing whitespace

This provides a quick first check that avoids the need for executing queries when there's an exact match.

### 2. Result Matching (CheckType.Compare)

If string comparison fails, the system compares the result set of the user's query with the result set of the reference solution:

```csharp
// Execute both queries and compare results
return await _sqlExecutionService.CheckByExecutionAsync(
    submittedQuery, 
    exercise.SolutionQuery, 
    exercise.Schema, 
    exercise.CheckQueryInsert);
```

Key considerations:
- Column order might be different but still correct
- Result sets must have the same number of rows and columns
- Data types must match between results

### 3. Select Validation (CheckType.Select)

This method validates the user's query against a predefined select query:

```csharp
// Just execute the user's query and compare with solution output
return await _sqlExecutionService.CheckByExecutionAsync(
    submittedQuery, 
    exercise.CheckQuerySelect, 
    exercise.Schema);
```

This is useful when:
- The exercise requires specific results regardless of query structure
- Multiple valid solutions might exist for the exercise

### 4. Insert and Select Validation (CheckType.InsertAndSelect)

This method validates the user's INSERT query by executing it and then running a SELECT query to check the results:

```csharp
// Execute user's insert query, then check with select
return await _sqlExecutionService.CheckByExecutionAsync(
    exercise.CheckQuerySelect, 
    exercise.CheckQuerySelect, 
    exercise.Schema, 
    submittedQuery);
```

This is used for:
- Exercises that require data modification
- Testing INSERT, UPDATE, or DELETE operations

## Safe Execution Environment

To prevent harmful queries, the system implements several safety measures:

### Query Sanitization

```csharp
// Example of query sanitization
public string SanitizeQuery(string query)
{
    // Block dangerous operations
    if (ContainsDataModification(query) || 
        ContainsSchemaModification(query) ||
        ContainsSensitiveTableAccess(query))
    {
        throw new UnsafeQueryException("Query contains unsafe operations");
    }
    
    return query;
}
```

### Isolated Execution Environment

- Each user query executes in a transaction that is rolled back
- Temporary database instances or schemas for execution
- Time limits on query execution
- Memory usage constraints

## Advanced Features

### Dynamic Result Validation

For exercises with multiple valid solutions, the system can validate against a set of validation rules rather than a single expected result.

```csharp
// Example of rule-based validation
public bool ValidateWithRules(string userQuery, List<ValidationRule> rules)
{
    var results = _queryExecutor.Execute(userQuery);
    
    return rules.All(rule => rule.Validate(results));
}
```

### Custom Query Transformations

To handle edge cases and different SQL dialects:

```csharp
// Example of query transformation
public string TransformQuery(string query, string dialectFrom, string dialectTo)
{
    return _dialectTransformer.Transform(query, dialectFrom, dialectTo);
}
```

## Integration with Exercises

Exercises define their validation criteria:

```json
{
  "id": "ex-123",
  "title": "Select All Customers",
  "description": "Write a query to select all customers from the Customers table.",
  "checkType": "ResultMatch",
  "solutionQuery": "SELECT * FROM Customers",
  "validationCriteria": {
    "requiredTables": ["Customers"],
    "requiredClauses": ["SELECT"]
  }
}
```

## Feedback Generation

When a solution is incorrect, the system provides constructive feedback:

```csharp
// Example feedback generation
public string GenerateFeedback(ValidationResult result)
{
    if (result.IsValid)
    {
        return "Correct solution!";
    }
    
    string feedback = "Your solution is incorrect. ";
    
    if (result.MissingColumns.Any())
    {
        feedback += $"Your result is missing columns: {string.Join(", ", result.MissingColumns)}. ";
    }
    
    if (result.ExtraRows.Any())
    {
        feedback += "Your result has extra rows that shouldn't be included. ";
    }
    
    return feedback;
}
```

## Error Handling

The system handles various error scenarios:

1. **Syntax Errors**: Provides clear error messages for SQL syntax issues
2. **Runtime Errors**: Catches execution errors and provides meaningful explanations
3. **Timeout Errors**: Handles long-running queries that exceed time limits
4. **Security Violations**: Blocks attempts to execute unauthorized operations

## Performance Optimizations

To ensure the system can handle many concurrent solution submissions:

1. **Query Caching**: Cache results of common queries
2. **Connection Pooling**: Efficient database connection management
3. **Parallel Execution**: Process multiple solution checks simultaneously
4. **Execution Plans**: Analyze and optimize query execution plans

## Future Enhancements

Planned improvements to the solution checking system:

1. **Machine Learning Validation**: Use ML to recognize alternative correct solutions
2. **Natural Language Feedback**: More human-readable feedback on errors
3. **Multi-Step Validation**: Support for exercises requiring multiple queries
4. **Visual Execution Plan Comparison**: Compare user query plan with optimal plan

## Troubleshooting

Common issues and solutions:

| Issue | Potential Cause | Solution |
|-------|----------------|----------|
| False negatives (correct solutions marked wrong) | Strict comparison logic | Review and adjust comparison logic |
| False positives (incorrect solutions marked correct) | Insufficient validation criteria | Add additional validation rules |
| Slow validation | Inefficient test data or complex comparison | Optimize test data size or comparison algorithms |
| Inconsistent results | Database state changes between validations | Ensure consistent database state via transactions |

## References

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [SQL Query Parsing Techniques](https://www.microsoft.com/en-us/research/publication/query-parsing-techniques/)
- [Database Query Performance Analysis](https://use-the-index-luke.com/)
- [SQL Injection Prevention](https://owasp.org/www-community/attacks/SQL_Injection) 