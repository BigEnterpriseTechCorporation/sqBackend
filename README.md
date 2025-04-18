# WIP: SQL Learning Platform API

A modern RESTful API for an SQL learning platform that features different types of SQL exercises and educational units.

## Overview

This API provides a robust backend for an educational platform focused on teaching SQL. The platform supports:

- **5 Types of Exercises**:
  - SelectAnswer: Multiple-choice questions
  - FillMissingWords: Fill in the missing parts of SQL queries 
  - ConstructQuery: Drag-and-drop query building from components
  - SimpleQuery: Write simple SQL queries from scratch
  - ComplexQuery: Write complex SQL queries from scratch

- **Units**: Collections of exercises grouped by topic or difficulty

## Getting Started

### Prerequisites

- .NET 9.0
- PostgreSQL (for production) or in-memory database (for development)

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/BigEnterpriseTechCorporation/sqBackend.git
   ```

2. Navigate to the project directory
   ```bash
   cd sql-learning-platform
   ```

3. Build the project
   ```bash
   dotnet build
   ```

4. Run the project
   ```bash
   dotnet run --project WebApi
   ```

5. The API will be available at:
   - Development: `http://localhost:5000/`
   - Swagger UI: `http://localhost:5000/swagger`

## Project Structure

- **ApplicationCore**: Contains the domain models, interfaces and DTOs
- **Infrastructure**: Contains implementations of repositories and services
- **WebApi**: The API layer with controllers and configuration

## API Documentation

For detailed information about the available endpoints, request/response formats, and authentication, please see the [API Documentation](docs/api_documentation.md).

## Features

- JWT Authentication and Authorization
- RESTful API design
- Swagger documentation
- Soft deletion
- Rate limiting
- CORS support
- Docker support