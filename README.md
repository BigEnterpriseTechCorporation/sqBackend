# SQL Learning Platform

A modern web application designed to help users learn SQL through interactive exercises and practical challenges.

## Overview

The SQL Learning Platform provides a structured learning environment for SQL beginners and intermediate users. The platform offers a collection of learning units, each containing exercises of varying difficulty levels that help users practice their SQL skills in a safe, guided environment.

## Features

- **Structured Learning Path**: Organized units with progressive difficulty
- **Interactive SQL Exercises**: Hands-on practice with immediate feedback
- **Solution Validation**: Automated checking of SQL queries against expected results
- **User Progress Tracking**: Statistics on completed exercises and attempts
- **User Authentication**: Secure registration and login process

## Technology Stack

- **Backend**: ASP.NET Core 6.0 Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI

## Getting Started

For detailed setup instructions, please refer to the [Installation Guide](docs/installation.md).

### Quick Start

1. Clone the repository:
```bash
git clone https://github.com/yourusername/sql-learning-platform.git
cd sql-learning-platform
```

2. Set up the database:
```bash
dotnet ef database update --project Infrastructure --startup-project WebApi
```

3. Run the application:
```bash
cd WebApi
dotnet run
```

4. Access the API at `https://localhost:5001` or `http://localhost:5000`

## Documentation

- [API Documentation](docs/api_documentation.md): Detailed API endpoints and usage
- [Installation Guide](docs/installation.md): Setup and deployment instructions
- [Developer Guide](docs/developer_guide.md): Architecture and development workflow
- [Exercise Solutions API](docs/exercise_solutions_api.md): Solution submission and checking
- [Technical Guide: Solution Checking](docs/solution_checking_technical.md): How SQL solutions are validated

## Architecture

The project follows Clean Architecture principles with clear separation of concerns:

- **ApplicationCore**: Domain entities, interfaces, and business logic
- **Infrastructure**: Data access and external service implementations
- **WebApi**: Controllers, DTOs, and API configuration

## Contributing

We welcome contributions to the SQL Learning Platform! Please see our [Developer Guide](docs/developer_guide.md) for information on how to get started.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

For questions or support, please open an issue on the GitHub repository.