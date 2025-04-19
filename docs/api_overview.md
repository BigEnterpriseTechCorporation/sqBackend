# API Overview

This document provides a high-level overview of all API endpoints available in the SQL Learning Platform. For detailed documentation on specific endpoint groups, please refer to the linked dedicated documentation files.

## Base URL

- Development: `http://localhost:5001` or `https://localhost:7181`
- Production: `https://api.sqllearningplatform.com`

## Authentication

All protected endpoints require a JWT Bearer token in the Authorization header:

```
Authorization: Bearer {your_token}
```

Tokens can be obtained through the authentication endpoints described below.

## API Endpoints Summary

| Category | Description | Detailed Documentation |
|----------|-------------|------------------------|
| Authentication | User registration, login, token refresh | [Authentication API](#authentication-api) |
| Users | User management and profiles | [Users API](#users-api) |
| Units | Learning modules management | [Units API](#units-api) |
| Exercises | SQL exercises management | [Exercises API](#exercises-api) |
| Solutions | Exercise solutions and validation | [Exercise Solutions API](exercise_solutions_api.md) |

## Authentication API

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|--------------|
| POST | `/api/account/register` | Register a new user | No |
| POST | `/api/account/login` | Authenticate and get tokens | No |
| POST | `/api/account/refresh-token` | Get a new access token using refresh token | No |
| GET | `/api/account/self` | Get current user info | Yes |

## Users API

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|--------------|
| GET | `/api/users` | Get all users (admin only) | Yes (Admin) |
| GET | `/api/users/{id}` | Get user by ID | Yes |
| DELETE | `/api/users/{id}` | Delete a user | Yes (Admin) |
| PUT | `/api/users/{id}` | Update user details | Yes (Owner/Admin) |
| GET | `/api/users/stats` | Get current user statistics | Yes |

## Units API

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|--------------|
| GET | `/api/units` | Get all units | Yes |
| GET | `/api/units/{id}` | Get unit by ID | Yes |
| POST | `/api/units` | Create a new unit | Yes (Admin) |
| PUT | `/api/units/{id}` | Update a unit | Yes (Admin) |
| DELETE | `/api/units/{id}` | Delete a unit | Yes (Admin) |
| POST | `/api/units/{id}/likes` | Like/unlike a unit | Yes |
| GET | `/api/units/{id}/likes` | Check if user liked a unit | Yes |
| GET | `/api/units/{id}/progress` | Get user progress for a unit | Yes |

## Exercises API

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|--------------|
| GET | `/api/exercises` | Get all exercises | Yes |
| GET | `/api/exercises/{id}` | Get exercise by ID | Yes |
| POST | `/api/exercises` | Create a new exercise | Yes (Admin) |
| PUT | `/api/exercises/{id}` | Update an exercise | Yes (Admin) |
| DELETE | `/api/exercises/{id}` | Delete an exercise | Yes (Admin) |
| GET | `/api/exercises/unit/{unitId}` | Get exercises for a unit | Yes |

**Note**: Exercise responses include a `SolutionQuery` field in the ExerciseDto which contains the reference solution. Client applications should implement appropriate logic to determine when this solution should be displayed to users, typically after multiple failed attempts or after successful completion.

## Exercise Solutions API

See [Exercise Solutions API](exercise_solutions_api.md) for detailed documentation.

Key endpoints:

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|--------------|
| POST | `/api/exercise-solutions/{exerciseId}` | Submit a solution | Yes |
| GET | `/api/exercise-solutions/{exerciseId}/history` | Get solution history | Yes |
| GET | `/api/exercise-solutions/{exerciseId}/progress` | Get exercise progress | Yes |

## Response Formats

All API responses follow a consistent format:

### Success Response

```json
{
  "data": { /* Response data */ },
  "success": true,
  "message": null
}
```

### Error Response

```json
{
  "data": null,
  "success": false,
  "message": "Error message describing what went wrong",
  "errors": [ /* Detailed error information */ ]
}
```

## HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 422 | Validation Error |
| 429 | Too Many Requests |
| 500 | Server Error |

## Pagination

Endpoints that return collections support pagination with these query parameters:

| Parameter | Description | Default |
|-----------|-------------|---------|
| pageNumber | Page number (1-based) | 1 |
| pageSize | Number of items per page | 10 |

Example: `/api/units?pageNumber=2&pageSize=20`

## Filtering and Sorting

Many collection endpoints support filtering and sorting:

| Parameter | Description | Example |
|-----------|-------------|---------|
| sortBy | Field to sort by | `sortBy=createdAt` |
| sortOrder | Order direction (asc/desc) | `sortOrder=desc` |
| {field} | Filter by field value | `difficulty=easy` |

Example: `/api/exercises?difficulty=easy&sortBy=createdAt&sortOrder=desc`

## Rate Limiting

The API implements rate limiting to prevent abuse:

- Authentication endpoints: 5 requests per minute
- Regular endpoints: 60 requests per minute
- Solutions submission: 30 submissions per minute

When rate limits are exceeded, the API returns a 429 Too Many Requests response with a Retry-After header.

## API Versioning

API versioning is handled through the Accept header:

```
Accept: application/json;version=1.0
```

If not specified, the latest version is used by default.

## CORS

The API supports Cross-Origin Resource Sharing for specified origins configured in the application settings.

## API Documentation

For interactive API documentation, access the Swagger UI at:

- Development: `https://localhost:7181/swagger`
- Production: `https://api.sqllearningplatform.com/swagger` 