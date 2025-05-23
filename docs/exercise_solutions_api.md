# Exercise Solutions API

This document details the API endpoints for submitting and managing SQL exercise solutions in the SQL Learning Platform.

## Related Documentation

- [API Overview](api_overview.md): High-level summary of all API endpoints
- [Solution Checking Technical Guide](solution_checking_technical.md): Technical details on solution validation
- [Developer Guide](developer_guide.md): General development guidelines
- [User Guide](user_guide.md): End-user instructions for submitting solutions

## Solution Submission

### Submit Solution

Submit a solution for a specific exercise.

```
POST /api/exercise-solutions/{exerciseId}
```

#### Authorization

- Requires authentication (JWT Bearer token)

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| exerciseId | GUID | The unique identifier of the exercise |

#### Request Body

```json
{
  "query": "SELECT * FROM customers WHERE customer_id > 100"
}
```

| Field | Type | Description |
|-------|------|-------------|
| query | string | The SQL query submitted as the solution |

#### Response

**Status Code: 200 OK**

```json
{
  "isCorrect": true,
  "feedback": "Excellent work! Your solution is correct.",
  "executionTimeMs": 24,
  "attemptNumber": 2
}
```

**Status Code: 400 Bad Request**

```json
{
  "isCorrect": false,
  "feedback": "Your query has a syntax error: Unclosed parenthesis at line 1.",
  "errorDetails": "SQL Error: near \")\": syntax error",
  "attemptNumber": 1
}
```

#### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| isCorrect | boolean | Whether the solution is correct |
| feedback | string | Feedback on the solution |
| executionTimeMs | integer | The time taken to execute the query (ms) |
| attemptNumber | integer | The number of attempts for this exercise |
| errorDetails | string | Detailed error message (only when errors occur) |

### Reference Solution Access

The platform includes a mechanism to provide reference solutions to users, which is particularly useful for learning purposes. Reference solutions are stored in the `SolutionQuery` field of exercise entities and can be accessed via the Exercise API.

#### Accessing Reference Solutions

Reference solutions are included in the `ExerciseDto` returned by these endpoints:

```
GET /api/exercises/{id}
GET /api/exercises
```

The `SolutionQuery` field contains the correct SQL query that solves the exercise. Client applications should implement appropriate logic to control the visibility of this solution based on the user's interaction with the exercise.

#### Recommended Display Logic

Frontend applications should consider the following guidelines for displaying reference solutions:

1. **Hide Initially**: Do not show the solution when a user first accesses an exercise
2. **Show After Multiple Attempts**: Make the solution available after a user has made several unsuccessful attempts
3. **Show After Completion**: Always show the solution after a user has correctly solved the exercise
4. **Explicit Request**: Provide a "Show Solution" button that users can click when they're stuck

#### Example Client Implementation

```javascript
function shouldShowSolution(exercise, userAttempts, hasCompletedExercise) {
  if (hasCompletedExercise) {
    return true; // Always show solution after completion
  }
  
  if (userAttempts >= 3) {
    return true; // Show solution after 3 attempts
  }
  
  return false; // Hide solution otherwise
}
```

### Get User Exercise Solutions

Retrieve a user's solution history for a specific exercise.

```
GET /api/exercise-solutions/{exerciseId}/history
```

#### Authorization

- Requires authentication (JWT Bearer token)

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| exerciseId | GUID | The unique identifier of the exercise |

#### Query Parameters

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| pageSize | integer | Number of solutions per page | 10 |
| pageNumber | integer | Page number (starting from 1) | 1 |

#### Response

**Status Code: 200 OK**

```json
{
  "totalCount": 5,
  "pageSize": 10,
  "pageNumber": 1,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "query": "SELECT * FROM customers WHERE customer_id > 100",
      "isCorrect": true,
      "submittedAt": "2023-09-15T14:30:45Z",
      "executionTimeMs": 24
    },
    {
      "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "query": "SELECT * FROM customer WHERE id > 100",
      "isCorrect": false,
      "submittedAt": "2023-09-15T14:28:12Z",
      "executionTimeMs": 18
    }
  ]
}
```

## User Progress

### Get User Progress for an Exercise

Retrieve a user's progress on a specific exercise.

```
GET /api/exercise-solutions/{exerciseId}/progress
```

#### Authorization

- Requires authentication (JWT Bearer token)

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| exerciseId | GUID | The unique identifier of the exercise |

#### Response

**Status Code: 200 OK**

```json
{
  "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "completed": true,
  "attemptsCount": 3,
  "firstCompletedAt": "2023-09-15T14:30:45Z",
  "lastAttemptAt": "2023-09-15T14:30:45Z",
  "averageExecutionTimeMs": 22
}
```

### Get User Progress for a Unit

Retrieve a user's progress on all exercises in a unit.

```
GET /api/units/{unitId}/progress
```

#### Authorization

- Requires authentication (JWT Bearer token)

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| unitId | GUID | The unique identifier of the unit |

#### Response

**Status Code: 200 OK**

```json
{
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalExercises": 5,
  "completedExercises": 3,
  "completionPercentage": 60,
  "exerciseProgress": [
    {
      "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "completed": true,
      "attemptsCount": 3
    },
    {
      "exerciseId": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "completed": true,
      "attemptsCount": 1
    },
    {
      "exerciseId": "5fa85f64-5717-4562-b3fc-2c963f66afa8",
      "completed": true,
      "attemptsCount": 2
    },
    {
      "exerciseId": "6fa85f64-5717-4562-b3fc-2c963f66afa9",
      "completed": false,
      "attemptsCount": 4
    },
    {
      "exerciseId": "7fa85f64-5717-4562-b3fc-2c963f66afaa",
      "completed": false,
      "attemptsCount": 0
    }
  ]
}
```

## Administrator Endpoints

### Get All Solutions for an Exercise

Retrieve all users' solutions for a specific exercise (admin only).

```
GET /api/admin/exercise-solutions/{exerciseId}
```

#### Authorization

- Requires authentication (JWT Bearer token)
- Requires admin role

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| exerciseId | GUID | The unique identifier of the exercise |

#### Query Parameters

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| pageSize | integer | Number of solutions per page | 20 |
| pageNumber | integer | Page number (starting from 1) | 1 |
| onlyCorrect | boolean | Filter only correct solutions | false |

#### Response

**Status Code: 200 OK**

```json
{
  "totalCount": 125,
  "pageSize": 20,
  "pageNumber": 1,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
      "userName": "john.doe",
      "query": "SELECT * FROM customers WHERE customer_id > 100",
      "isCorrect": true,
      "submittedAt": "2023-09-15T14:30:45Z",
      "executionTimeMs": 24
    },
    // ... more solutions
  ]
}
```

### Get Solution Statistics for an Exercise

Retrieve statistics about solutions for a specific exercise (admin only).

```
GET /api/admin/exercise-solutions/{exerciseId}/statistics
```

#### Authorization

- Requires authentication (JWT Bearer token)
- Requires admin role

#### Path Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| exerciseId | GUID | The unique identifier of the exercise |

#### Response

**Status Code: 200 OK**

```json
{
  "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "totalAttempts": 325,
  "uniqueUsers": 145,
  "correctSolutions": 118,
  "incorrectSolutions": 207,
  "averageAttemptsUntilCorrect": 2.76,
  "medianExecutionTimeMs": 28,
  "commonErrors": [
    {
      "errorType": "Syntax error",
      "count": 78,
      "percentage": 37.7
    },
    {
      "errorType": "Wrong table name",
      "count": 45,
      "percentage": 21.7
    },
    {
      "errorType": "Missing columns",
      "count": 35,
      "percentage": 16.9
    }
  ]
}
```

## Error Codes

| Status Code | Description |
|-------------|-------------|
| 400 | Bad Request - Malformed request or invalid SQL query |
| 401 | Unauthorized - Authentication required |
| 403 | Forbidden - User doesn't have permission |
| 404 | Not Found - Exercise not found |
| 429 | Too Many Requests - Rate limit exceeded |
| 500 | Internal Server Error - Server-side issue |

## Rate Limiting

Exercise solution submissions are rate-limited to prevent abuse:

- 30 submissions per minute per user
- 500 submissions per day per user

If the rate limit is exceeded, the API will return a 429 Too Many Requests response with a Retry-After header indicating when the client can retry.

## Webhooks

The platform can notify external systems about solution events via webhooks.

Administrators can configure webhooks to receive notifications for:

- Solution submissions
- First successful solutions for exercises
- Unit completions

Webhook requests include an HMAC signature for verification.

## Error Handling Examples

### SQL Syntax Error

```json
{
  "status": 400,
  "title": "SQL Syntax Error",
  "detail": "Syntax error in SQL query: Unclosed parenthesis",
  "errorCode": "SQL_SYNTAX_ERROR",
  "errorPosition": {
    "line": 1,
    "column": 35
  }
}
```

### Timeout Error

```json
{
  "status": 400,
  "title": "Query Execution Timeout",
  "detail": "The query took too long to execute (exceeded 10 seconds)",
  "errorCode": "QUERY_TIMEOUT"
}
```

### Security Violation

```json
{
  "status": 400,
  "title": "Security Violation",
  "detail": "The query contains unauthorized operations",
  "errorCode": "SECURITY_VIOLATION",
  "violations": [
    "Attempted data modification (DELETE statement detected)",
    "Attempted to access system tables"
  ]
}
``` 