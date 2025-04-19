# Developer Guide

This guide provides information for developers working on the SQL Learning Platform, covering architecture, code organization, workflow, and best practices.

## Related Documentation

- [API Overview](api_overview.md): High-level summary of all API endpoints
- [Installation Guide](installation.md): Setup and deployment instructions
- [Solution Checking Technical Guide](solution_checking_technical.md): How SQL solutions are validated
- [Exercise Solutions API](exercise_solutions_api.md): API for exercise solution submission

## Architecture Overview

The SQL Learning Platform follows Clean Architecture principles with the following layers:

1. **Core Layers**
   - `ApplicationCore`: Contains domain entities, interfaces, and business logic
   - `Infrastructure`: Implements persistence, external services, and infrastructure concerns

2. **API Layer**
   - `WebApi`: Exposes REST endpoints, handles HTTP requests/responses, and manages authentication

### Domain Model

The core domain entities include:

- `User`: Represents a platform user with authentication details
- `Unit`: Represents a learning module containing multiple exercises
- `Exercise`: Represents a SQL challenge with different difficulty levels
- `UserSolution`: Tracks a user's solution to an exercise
- `UnitLike`: Tracks users' liked/favorited units

## Project Structure

```
sqBackend/
├── ApplicationCore/             # Domain models and business logic
│   ├── Entities/                # Domain entities
│   ├── Interfaces/              # Interfaces for services and repositories
│   └── Services/                # Business logic implementations
├── Infrastructure/              # Data persistence and external services
│   ├── Data/                    # Database context and configurations
│   │   ├── Configurations/      # Entity type configurations
│   │   └── Migrations/          # EF Core migrations
│   ├── Identity/                # Authentication implementation
│   └── Services/                # Service implementations
├── WebApi/                      # API controllers and configuration
│   ├── Controllers/             # API endpoints
│   ├── DTO/                     # Data transfer objects
│   ├── Filters/                 # Action filters and authorization
│   ├── Extensions/              # Service collection extensions
│   └── Middleware/              # Custom middleware components
└── Tests/                       # Test projects
    ├── UnitTests/               # Unit tests
    └── IntegrationTests/        # Integration tests
```

## Data Access

The project uses Entity Framework Core for data access with a PostgreSQL database:

- `ApplicationDbContext`: The main EF Core DbContext
- Repository pattern for data access abstraction
- Entity configurations in `Infrastructure/Data/Configurations`

### Entity Framework Migrations

To create a new migration:

```bash
dotnet ef migrations add MigrationName --project Infrastructure --startup-project WebApi
```

To apply migrations:

```bash
dotnet ef database update --project Infrastructure --startup-project WebApi
```

## Authentication and Authorization

The application uses JWT-based authentication:

- JSON Web Tokens (JWT) for stateless authentication
- Refresh tokens for extended sessions
- Role-based authorization with user roles
- Authorization policies for controller methods

The `AccountController` handles user registration, login, and token refresh operations.

## API Design

The API follows RESTful principles:

- Resources are identified by URLs
- HTTP methods define the operation (GET, POST, PUT, DELETE)
- JSON response format
- HTTP status codes indicate success/failure

### Controllers Overview

- `AccountController`: User registration, authentication, token management
- `UsersController`: User management operations
- `UnitsController`: Learning unit management
- `ExercisesController`: Exercise management and solution checking

## SQL Solution Checking

The platform includes a SQL solution checker that:

1. Executes user-submitted SQL queries in a safe environment
2. Compares results with expected outputs
3. Validates against exercise requirements
4. Provides appropriate feedback

## Development Workflow

### Setting Up Development Environment

1. Follow the installation guide to set up dependencies
2. Configure user secrets for local development:
   ```bash
   dotnet user-secrets init --project WebApi
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=sql_learning;Username=sql_app;Password=your_password" --project WebApi
   ```

### Coding Conventions

- Follow C# coding conventions and Microsoft .NET design guidelines
- Use meaningful names for classes, methods, and variables
- Document public APIs with XML comments
- Use async/await for asynchronous operations
- Implement proper exception handling

### Testing

The project includes several testing approaches:

1. **Unit Tests**: Test individual components in isolation
   ```bash
   dotnet test Tests/UnitTests
   ```

2. **Integration Tests**: Test interactions between components
   ```bash
   dotnet test Tests/IntegrationTests
   ```

### Code Reviews

For code reviews, focus on:

- Functional correctness
- Security implications
- Performance considerations
- Adherence to project patterns
- Test coverage

## Working with Frontend

The SQL Learning Platform's frontend (separate repository) communicates with this API using:

- RESTful API calls
- JWT Bearer tokens for authentication
- JSON data exchange

When making API changes:
1. Update the API documentation
2. Consider backward compatibility
3. Notify frontend developers of breaking changes

## Performance Considerations

- Use asynchronous methods for I/O operations
- Implement appropriate database indexes
- Consider pagination for large result sets
- Use eager loading to prevent N+1 query problems

## Security Best Practices

- Validate all user inputs
- Sanitize SQL queries to prevent injection attacks
- Use HTTPS in all environments
- Implement proper authorization checks
- Store sensitive information in user secrets or environment variables
- Regularly update dependencies

## Debugging

- Use logging with appropriate log levels
- API includes Swagger for endpoint testing
- Configure development environment for debugging:
  ```json
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  }
  ```

## Common Development Tasks

### Adding a New Entity

1. Create entity class in `ApplicationCore/Entities`
2. Add DbSet to `ApplicationDbContext`
3. Create entity configuration in `Infrastructure/Data/Configurations`
4. Add repository interface and implementation
5. Create service interface and implementation
6. Create DTOs in `WebApi/DTO`
7. Create API controller
8. Generate and apply EF migration
9. Update API documentation

### Adding a New API Endpoint

1. Identify the appropriate controller
2. Define required DTOs
3. Implement the endpoint method with proper HTTP verb
4. Add appropriate authorization attributes
5. Implement validation
6. Document the endpoint in API documentation
7. Test the endpoint with Swagger

## Troubleshooting Development Issues

### Common Issues

1. **Entity Framework Migrations**
   - If migrations fail, check the database provider connection
   - Ensure all entity properties have appropriate types

2. **Authentication Problems**
   - Check JWT token settings in configuration
   - Verify client sends token correctly
   - Check token expiration settings

3. **Database Connection**
   - Verify connection string format
   - Check database user permissions
   - Ensure PostgreSQL service is running

4. **Performance Issues**
   - Use SQL Server Profiler to identify slow queries
   - Review EF Core query execution plans
   - Check for missing indexes

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core)
- [JWT Authentication in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) 