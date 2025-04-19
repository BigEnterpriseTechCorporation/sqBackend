# SQL Learning API Documentation

This document provides comprehensive information about the SQL Learning API, which offers endpoints for managing units (collections of exercises) and SQL exercises with different types of challenges.

## Authentication

The API uses JWT Bearer authentication. To access protected endpoints, you need to:

1. Obtain a token using the login endpoint
2. Include the token in the Authorization header of your requests:
   ```
   Authorization: Bearer your_token_here
   ```

## Base URL

- Development: `http://localhost:port/`
- Production: `https://rpi.tail707b9c.ts.net/api/v1/`

## Resource: Units

Units are collections of exercises that can be used to organize learning material.

### Endpoints

#### Get All Units

```
GET /api/units
```

Returns a list of all units in the system.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "createdAt": "2023-05-20T10:00:00Z",
    "updatedAt": "2023-05-21T15:30:00Z",
    "title": "SQL Basics",
    "description": "Learn the basics of SQL",
    "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afb7",
    "ownerName": "John Doe",
    "exerciseCount": 5
  },
  ...
]
```

#### Get Unit By ID

```
GET /api/units/{id}
```

Returns details of a specific unit, including all exercises it contains.

**Parameters**
- `id` (path, guid, required): The ID of the unit to retrieve

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "createdAt": "2023-05-20T10:00:00Z",
  "updatedAt": "2023-05-21T15:30:00Z",
  "title": "SQL Basics",
  "description": "Learn the basics of SQL",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afb7",
  "ownerName": "John Doe",
  "exerciseCount": 2,
  "exercises": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afc8",
      "createdAt": "2023-05-20T11:00:00Z",
      "updatedAt": "2023-05-21T16:30:00Z",
      "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "unitTitle": "SQL Basics",
      "title": "Select all customers",
      "description": "Write a query to select all customers",
      "type": "SimpleQuery",
      "difficulty": "Easy",
      "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
      "checkType": "Compare",
      "checkQueryInsert": "",
      "checkQuerySelect": "",
      "options": "",
      "queryParts": ""
    },
    ...
  ]
}
```

#### Get Unit Exercises

```
GET /api/units/{unitId}/exercises
```

Returns a list of all exercises in a specific unit.

**Parameters**
- `unitId` (path, guid, required): The ID of the unit

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afc8",
    "createdAt": "2023-05-20T11:00:00Z",
    "updatedAt": "2023-05-21T16:30:00Z",
    "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "unitTitle": "SQL Basics",
    "title": "Select all customers",
    "description": "Write a query to select all customers",
    "type": "SimpleQuery",
    "difficulty": "Easy",
    "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
    "checkType": "Compare",
    "checkQueryInsert": "",
    "checkQuerySelect": "",
    "options": "",
    "queryParts": ""
  },
  ...
]
```

#### Create Unit

```
POST /api/units
```

Creates a new unit. Requires authentication.

**Request Body**
- Content-Type: application/json

```json
{
  "title": "SQL Basics",
  "description": "Learn the basics of SQL"
}
```

**Response**
- Status: 201 Created
- Content-Type: application/json

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "createdAt": "2023-05-20T10:00:00Z",
  "updatedAt": null,
  "title": "SQL Basics",
  "description": "Learn the basics of SQL",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afb7",
  "ownerName": "John Doe",
  "exerciseCount": 0
}
```

#### Update Unit

```
PUT /api/units/{id}
```

Updates an existing unit. Requires authentication and ownership of the unit.

**Parameters**
- `id` (path, guid, required): The ID of the unit to update

**Request Body**
- Content-Type: application/json

```json
{
  "title": "Updated SQL Basics",
  "description": "Updated description for SQL basics"
}
```

**Response**
- Status: 204 No Content

#### Delete Unit

```
DELETE /api/units/{id}
```

Deletes a unit. Requires authentication and ownership of the unit.

**Parameters**
- `id` (path, guid, required): The ID of the unit to delete

**Response**
- Status: 204 No Content

## Resource: Exercises

Exercises are SQL challenges with different types and difficulty levels.

### Exercise Types

The API supports 5 types of exercises:

1. `SelectAnswer` - Multiple-choice question where the user selects from provided options
2. `FillMissingWords` - Fill in missing parts in a SQL query or text
3. `ConstructQuery` - Build a query from given parts/words
4. `SimpleQuery` - Write a simple SQL query from scratch
5. `ComplexQuery` - Write a complex SQL query from scratch

### Endpoints

#### Get All Exercises

```
GET /api/exercises
```

Returns a list of all exercises in the system.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afc8",
    "createdAt": "2023-05-20T11:00:00Z",
    "updatedAt": "2023-05-21T16:30:00Z",
    "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "unitTitle": "SQL Basics",
    "title": "Select all customers",
    "description": "Write a query to select all customers",
    "type": "SimpleQuery",
    "difficulty": "Easy",
    "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
    "checkType": "Compare",
    "checkQueryInsert": "",
    "checkQuerySelect": "",
    "options": "",
    "queryParts": ""
  },
  ...
]
```

#### Get Exercise By ID

```
GET /api/exercises/{id}
```

Returns details of a specific exercise.

**Parameters**
- `id` (path, guid, required): The ID of the exercise to retrieve

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afc8",
  "createdAt": "2023-05-20T11:00:00Z",
  "updatedAt": "2023-05-21T16:30:00Z",
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "unitTitle": "SQL Basics",
  "title": "Select all customers",
  "description": "Write a query to select all customers",
  "type": "SimpleQuery",
  "difficulty": "Easy",
  "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
  "checkType": "Compare",
  "checkQueryInsert": "",
  "checkQuerySelect": "",
  "options": "",
  "queryParts": ""
}
```

#### Create Exercise

```
POST /api/exercises
```

Creates a new exercise. Requires authentication and ownership of the unit.

**Request Body**
- Content-Type: application/json

Different types of exercises require different fields:

**SimpleQuery or ComplexQuery**:
```json
{
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Select all customers",
  "description": "Write a query to select all customers",
  "type": "SimpleQuery",
  "difficulty": "Easy",
  "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
  "checkType": "Compare",
  "solutionQuery": "SELECT * FROM Customers"
}
```

**SelectAnswer**:
```json
{
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Choose the correct query",
  "description": "Which query selects all customers?",
  "type": "SelectAnswer",
  "difficulty": "Easy",
  "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
  "options": "[\"SELECT * FROM Customers\", \"SELECT ID FROM Customers\", \"SELECT Name FROM Customers\", \"SELECT COUNT(*) FROM Customers\"]",
  "solutionQuery": "SELECT * FROM Customers"
}
```

**FillMissingWords**:
```json
{
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Fill in the missing parts",
  "description": "Complete the query to select all customers",
  "type": "FillMissingWords",
  "difficulty": "Easy",
  "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
  "options": "[\"SELECT\", \"*\", \"FROM\", \"Customers\", \"WHERE\"]",
  "solutionQuery": "SELECT * FROM Customers"
}
```

**ConstructQuery**:
```json
{
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Build a query",
  "description": "Construct a query to select all customers",
  "type": "ConstructQuery",
  "difficulty": "Easy",
  "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
  "queryParts": "[\"SELECT\", \"*\", \"FROM\", \"Customers\", \"WHERE\", \"ID\", \"=\", \"1\"]",
  "solutionQuery": "SELECT * FROM Customers WHERE ID = 1"
}
```

**Response**
- Status: 201 Created
- Content-Type: application/json

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afc8",
  "createdAt": "2023-05-20T11:00:00Z",
  "updatedAt": null,
  "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "unitTitle": "SQL Basics",
  "title": "Select all customers",
  "description": "Write a query to select all customers",
  "type": "SimpleQuery",
  "difficulty": "Easy",
  "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
  "checkType": "Compare",
  "checkQueryInsert": "",
  "checkQuerySelect": "",
  "options": "",
  "queryParts": ""
}
```

#### Update Exercise

```
PUT /api/exercises/{id}
```

Updates an existing exercise. Requires authentication and ownership of the unit.

**Parameters**
- `id` (path, guid, required): The ID of the exercise to update

**Request Body**
- Content-Type: application/json

The request body is similar to the Create Exercise endpoint.

**Response**
- Status: 204 No Content

#### Delete Exercise

```
DELETE /api/exercises/{id}
```

Deletes an exercise. Requires authentication and ownership of the unit.

**Parameters**
- `id` (path, guid, required): The ID of the exercise to delete

**Response**
- Status: 204 No Content

## Models

### Unit

| Field       | Type     | Description                              |
|-------------|----------|------------------------------------------|
| id          | Guid     | Unique identifier                         |
| createdAt   | DateTime | Creation timestamp                       |
| updatedAt   | DateTime | Last update timestamp                    |
| title       | string   | Title of the unit                        |
| description | string   | Description of the unit                  |
| ownerId     | Guid     | ID of the user who created the unit      |
| ownerName   | string   | Name of the user who created the unit    |
| exercises   | Exercise[] | List of exercises in the unit          |

### Exercise

| Field           | Type         | Description                                                 |
|-----------------|--------------|-------------------------------------------------------------|
| id              | Guid         | Unique identifier                                            |
| createdAt       | DateTime     | Creation timestamp                                          |
| updatedAt       | DateTime     | Last update timestamp                                       |
| title           | string       | Title of the exercise                                       |
| description     | string       | Description of the exercise                                 |
| unitId          | Guid         | ID of the parent unit                                       |
| unitTitle       | string       | Title of the parent unit                                    |
| type            | ExerciseType | Type of exercise                                            |
| difficulty       | Difficulty    | Difficulty level                                             |
| schema          | string       | SQL schema definition                                        |
| checkType       | CheckType    | Method for checking exercise                                |
| checkQueryInsert| string       | Query for inserting test data                               |
| checkQuerySelect| string       | Query for selecting test results                            |
| solutionQuery   | string       | Correct solution query                                      |
| options         | string       | JSON array of options for SelectAnswer and FillMissingWords |
| queryParts      | string       | JSON array of query parts for ConstructQuery                |

### Enums

#### ExerciseType
- `SelectAnswer` - Multiple-choice question
- `FillMissingWords` - Fill in the blanks
- `ConstructQuery` - Build from components
- `SimpleQuery` - Write a simple query
- `ComplexQuery` - Write a complex query

#### Difficulty
- `Easy`
- `Normal`
- `Hard`
- `UltraHard`

#### CheckType
- `Compare` - Compare with solution output
- `Select` - Run select only
- `InsertAndSelect` - Run insert and select 

## Resource: Account

The Account resource handles user authentication and account management.

### Endpoints

#### Register

```
POST /Account/register
```

Creates a new user account.

**Request Body**
- Content-Type: application/json

```json
{
  "UserName": "johndoe",
  "FullName": "John Doe",
  "Password": "SecurePassword123"
}
```

**Response**
- Status: 201 Created
- Content-Type: application/json

```json
{
  "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Role": "User",
  "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Login

```
POST /Account/login
```

Authenticates a user and returns an access token.

**Request Body**
- Content-Type: application/json

```json
{
  "username": "johndoe",
  "password": "SecurePassword123"
}
```

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "Id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "Role": "User",
  "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Refresh Token

```
GET /Account/refresh
```

Refreshes the user's authentication token. Requires authentication.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### Get Current User

```
GET /Account/self
```

Returns the currently authenticated user's information. Requires authentication.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userName": "johndoe",
  "fullName": "John Doe",
  "email": "johndoe@localhost",
  "role": "User"
}
```

#### Update User Profile

```
PUT /Account/profile
```

Updates the currently authenticated user's profile information. Requires authentication.

**Request Body**
- Content-Type: application/json

```json
{
  "UserName": "johndoe2",
  "FullName": "John D. Doe",
  "CurrentPassword": "CurrentPassword123",
  "NewPassword": "NewPassword123"
}
```

> Note: All fields except `CurrentPassword` are optional, but at least one of `UserName`, `FullName`, or `NewPassword` must be provided.

**Response**
- Status: 200 OK
- Content-Type: application/json

If username or password were changed, a new token is returned:

```json
{
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "johndoe2",
    "fullName": "John D. Doe",
    "email": "johndoe2@localhost",
    "role": "User"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

If only full name was changed:

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userName": "johndoe",
  "fullName": "John D. Doe",
  "email": "johndoe@localhost",
  "role": "User"
}
```

## Resource: Users

The Users resource provides endpoints for managing users in the system.

### Endpoints

#### Get All Users

```
GET /api/users
```

Returns a list of all users in the system. Requires admin authorization.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userName": "johndoe",
    "fullName": "John Doe",
    "email": "johndoe@localhost",
    "role": "User"
  },
  ...
]
```

#### Get User By ID

```
GET /api/users/{id}
```

Returns details of a specific user.

**Parameters**
- `id` (path, guid, required): The ID of the user to retrieve

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userName": "johndoe",
  "fullName": "John Doe",
  "email": "johndoe@localhost",
  "role": "User"
}
```

#### Delete User

```
DELETE /api/users/{id}
```

Deletes a specific user. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the user to delete

**Response**
- Status: 204 No Content

## Resource: Admin

The Admin resource provides endpoints for system administration tasks.

### Endpoints

#### Get All Users (Admin)

```
GET /api/admin/users
```

Returns a paginated list of all users with filtering options. Requires admin authorization.

**Parameters**
- `role` (query, string, optional): Filter users by role (e.g., "Admin", "Member")
- `searchTerm` (query, string, optional): Search users by username or full name
- `page` (query, integer, optional): Page number (1-based, default: 1)
- `pageSize` (query, integer, optional): Page size (default: 20, max: 100)

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userName": "johndoe",
      "fullName": "John Doe",
      "email": "johndoe@localhost",
      "role": "Member",
      "isActive": true,
      "createdAt": "2023-06-15T10:00:00Z",
      "updatedAt": "2023-06-16T15:30:00Z"
    },
    ...
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 45,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### Update User Role

```
PUT /api/admin/users/{id}/role
```

Updates a user's role. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the user to update

**Request Body**
- Content-Type: application/json

```json
{
  "role": "Admin"
}
```

**Response**
- Status: 204 No Content

#### Toggle User Active Status

```
PUT /api/admin/users/{id}/status
```

Activates or deactivates a user account. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the user to update

**Request Body**
- Content-Type: application/json

```json
{
  "isActive": true
}
```

**Response**
- Status: 204 No Content

#### Get All Units (Admin)

```
GET /api/admin/units
```

Returns a paginated list of all units with filtering options. Requires admin authorization.

**Parameters**
- `ownerId` (query, guid, optional): Filter units by owner ID
- `searchTerm` (query, string, optional): Search units by title or description
- `page` (query, integer, optional): Page number (1-based, default: 1)
- `pageSize` (query, integer, optional): Page size (default: 20, max: 100)

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "createdAt": "2023-05-20T10:00:00Z",
      "updatedAt": "2023-05-21T15:30:00Z",
      "title": "SQL Basics",
      "description": "Learn the basics of SQL",
      "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afb7",
      "ownerName": "John Doe",
      "isActive": true,
      "exerciseCount": 5
    },
    ...
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 12,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

#### Toggle Unit Active Status

```
PUT /api/admin/units/{id}/status
```

Activates or deactivates a unit. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the unit to update

**Request Body**
- Content-Type: application/json

```json
{
  "isActive": true
}
```

**Response**
- Status: 204 No Content

#### Delete Unit (Admin Override)

```
DELETE /api/admin/units/{id}
```

Deletes a unit, bypassing ownership restrictions. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the unit to delete

**Response**
- Status: 204 No Content

#### Get All Exercises (Admin)

```
GET /api/admin/exercises
```

Returns a paginated list of all exercises with filtering options. Requires admin authorization.

**Parameters**
- `unitId` (query, guid, optional): Filter exercises by unit ID
- `type` (query, string, optional): Filter by exercise type (e.g., "SimpleQuery", "SelectAnswer")
- `difficulty` (query, string, optional): Filter by difficulty (e.g., "Easy", "Hard")
- `searchTerm` (query, string, optional): Search exercises by title or description
- `page` (query, integer, optional): Page number (1-based, default: 1)
- `pageSize` (query, integer, optional): Page size (default: 20, max: 100)

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afc8",
      "createdAt": "2023-05-20T11:00:00Z",
      "updatedAt": "2023-05-21T16:30:00Z",
      "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "unitTitle": "SQL Basics",
      "title": "Select all customers",
      "description": "Write a query to select all customers",
      "type": "SimpleQuery",
      "difficulty": "Easy",
      "isActive": true,
      "schema": "CREATE TABLE Customers (ID int, Name varchar(255))",
      "checkType": "Compare",
      "checkQueryInsert": "",
      "checkQuerySelect": "",
      "options": "",
      "queryParts": ""
    },
    ...
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 68,
  "totalPages": 4,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### Toggle Exercise Active Status

```
PUT /api/admin/exercises/{id}/status
```

Activates or deactivates an exercise. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the exercise to update

**Request Body**
- Content-Type: application/json

```json
{
  "isActive": true
}
```

**Response**
- Status: 204 No Content

#### Delete Exercise (Admin Override)

```
DELETE /api/admin/exercises/{id}
```

Deletes an exercise, bypassing ownership restrictions. Requires admin authorization.

**Parameters**
- `id` (path, guid, required): The ID of the exercise to delete

**Response**
- Status: 204 No Content

#### Get System Statistics

```
GET /api/admin/stats
```

Returns system-wide statistics. Requires admin authorization.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "totalUsers": 45,
  "activeUsers": 42,
  "adminUsers": 3,
  "usersByCreationDate": [
    {
      "date": "2023-06-01T00:00:00Z",
      "count": 5
    },
    {
      "date": "2023-06-02T00:00:00Z",
      "count": 8
    },
    ...
  ],
  "totalUnits": 12,
  "totalExercises": 68,
  "exercisesByType": {
    "SimpleQuery": 20,
    "ComplexQuery": 15,
    "SelectAnswer": 18,
    "FillMissingWords": 10,
    "ConstructQuery": 5
  },
  "exercisesByDifficulty": {
    "Easy": 25,
    "Normal": 20,
    "Hard": 15,
    "UltraHard": 8
  }
}
```

## Resource: Exercise Solutions

Exercise Solutions represent user submissions to exercises and track progress.

### Endpoints

#### Submit Solution

```
POST /api/ExerciseSolutions/{exerciseId}
```

Submit a solution for an exercise and verify if it's correct. Requires authentication.

**Parameters**
- `exerciseId` (path, guid, required): The ID of the exercise to solve

**Request Body**
- Content-Type: application/json

```json
{
  "query": "SELECT * FROM users WHERE age > 18"
}
```

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "isCorrect": true,
  "attemptCount": 2,
  "exerciseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "feedback": null
}
```

#### Get User Statistics

```
GET /api/ExerciseSolutions/stats
```

Get statistics about the current user's progress. Requires authentication.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "username": "johndoe",
  "totalExercises": 50,
  "solvedExercises": 25,
  "totalAttempts": 75,
  "likedUnits": 10,
  "completionPercentage": 50.0
}
```

#### Get Solved Exercises

```
GET /api/ExerciseSolutions/solved
```

Get a list of exercises the current user has solved correctly. Requires authentication.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "createdAt": "2023-01-01T12:00:00Z",
    "updatedAt": "2023-01-02T12:00:00Z",
    "unitId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "unitTitle": "SQL Basics",
    "title": "Simple SELECT Query",
    "description": "Write a query to select all users",
    "difficulty": 0,
    "type": 3,
    "schema": "CREATE TABLE users (id INT, name TEXT, age INT);",
    "checkType": 0,
    "checkQueryInsert": "INSERT INTO users VALUES (1, 'John', 25), (2, 'Jane', 30);",
    "checkQuerySelect": "SELECT * FROM users;",
    "options": "",
    "queryParts": ""
  },
  ...
]
```

#### Get Unsolved Exercises

```
GET /api/ExerciseSolutions/unsolved
```

Get a list of exercises the current user has not yet solved correctly. Requires authentication.

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
[
  // Same format as solved exercises
]
```

## Resource: Unit Likes

Unit Likes represent user preferences for units.

### Endpoints

#### Toggle Like for a Unit

```
POST /api/units/{unitId}/likes
```

Like or unlike a unit (toggles the current state). Requires authentication.

**Parameters**
- `unitId` (path, guid, required): The ID of the unit to like/unlike

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "isLiked": true,
  "likesCount": 42
}
```

#### Get Like Status for a Unit

```
GET /api/units/{unitId}/likes
```

Check if the current user has liked a specific unit. Requires authentication.

**Parameters**
- `unitId` (path, guid, required): The ID of the unit to check

**Response**
- Status: 200 OK
- Content-Type: application/json

```json
{
  "isLiked": true,
  "likesCount": 42
}
```

## Data Models

### Solution Result

```json
{
  "isCorrect": true,              // Whether the solution is correct
  "attemptCount": 2,              // Number of attempts made on this exercise
  "exerciseId": "guid",           // ID of the exercise
  "userId": "guid",               // ID of the user
  "feedback": "string or null"    // Optional feedback message
}
```

### User Exercise Stats

```json
{
  "userId": "guid",               // User ID
  "username": "string",           // Username
  "totalExercises": 50,           // Total number of exercises in the system
  "solvedExercises": 25,          // Number of exercises solved by the user
  "totalAttempts": 75,            // Total number of attempts made by the user
  "likedUnits": 10,               // Number of units liked by the user
  "completionPercentage": 50.0    // Percentage of exercises completed
}
```

### Like Result

```json
{
  "isLiked": true,                // Whether the user has liked the unit
  "likesCount": 42                // Total number of likes for the unit
}
``` 