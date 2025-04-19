# Installation Guide

This guide provides detailed instructions for setting up the SQL Learning Platform in both development and production environments.

## Related Documentation

- [Developer Guide](developer_guide.md): Architecture and development workflow
- [API Overview](api_overview.md): Summary of API endpoints
- [User Guide](user_guide.md): End-user instructions

## Development Environment Setup

### Prerequisites

- .NET 6.0 SDK or newer
- PostgreSQL 13.0 or newer
- Git
- IDE (Visual Studio 2022, JetBrains Rider, or VS Code recommended)

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/sqBackend.git
cd sqBackend
```

### Step 2: Database Setup

1. Install PostgreSQL if not already installed:
   - [PostgreSQL Downloads](https://www.postgresql.org/download/)

2. Create a new database and user:
   ```sql
   CREATE DATABASE sql_learning;
   CREATE USER sql_app WITH ENCRYPTED PASSWORD 'your_secure_password';
   GRANT ALL PRIVILEGES ON DATABASE sql_learning TO sql_app;
   ```

3. Update connection string in `WebApi/appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=sql_learning;Username=sql_app;Password=your_secure_password"
   }
   ```

### Step 3: Apply Database Migrations

```bash
cd WebApi
dotnet ef database update
```

If you encounter any issues with Entity Framework tools, install them first:
```bash
dotnet tool install --global dotnet-ef
```

### Step 4: Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7181`
- HTTP: `http://localhost:5181`
- Swagger UI: `https://localhost:7181/swagger`

## Production Deployment

### Option 1: Docker Deployment

1. Install Docker and Docker Compose
   - [Docker Installation](https://docs.docker.com/get-docker/)
   - [Docker Compose Installation](https://docs.docker.com/compose/install/)

2. Build the Docker image:
   ```bash
   docker build -t sql-learning-api -f Dockerfile .
   ```

3. Create a `docker-compose.yml` file:
   ```yaml
   version: '3.8'
   
   services:
     api:
       image: sql-learning-api:latest
       ports:
         - "8080:80"
       environment:
         - ASPNETCORE_ENVIRONMENT=Production
         - ConnectionStrings__DefaultConnection=Host=db;Database=sql_learning;Username=sql_app;Password=your_secure_password
       depends_on:
         - db
       restart: always
     
     db:
       image: postgres:13
       volumes:
         - pgdata:/var/lib/postgresql/data
       environment:
         - POSTGRES_DB=sql_learning
         - POSTGRES_USER=sql_app
         - POSTGRES_PASSWORD=your_secure_password
       ports:
         - "5432:5432"
       restart: always
   
   volumes:
     pgdata:
   ```

4. Start the containers:
   ```bash
   docker-compose up -d
   ```

### Option 2: Hosting on IIS

1. Install the [.NET 6.0 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/6.0)

2. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. In IIS:
   - Create a new website
   - Set the physical path to the publish folder
   - Configure the application pool to use No Managed Code
   - Set up HTTPS bindings

4. Update the `appsettings.Production.json` file with appropriate connection strings and settings

### Option 3: Azure App Service

1. Create an Azure App Service and Azure Database for PostgreSQL

2. Set up connection strings in Azure App Service Configuration:
   ```
   ConnectionStrings__DefaultConnection=Host=your-server.postgres.database.azure.com;Database=sql_learning;Username=your_admin@your-server;Password=your_password
   ```

3. Deploy using Visual Studio or Azure DevOps:
   ```bash
   dotnet publish -c Release
   # Use Azure CLI or VS deployment tools to push the published app
   ```

## Configuration Options

### Authentication Settings

In `appsettings.json`:
```json
"JwtConfig": {
  "Secret": "your-very-long-secret-key-at-least-32-characters",
  "TokenLifespan": "01:00:00",
  "RefreshTokenLifespan": "7.00:00:00"
}
```

### CORS Configuration

In `appsettings.json`:
```json
"CorsOrigins": [
  "http://localhost:3000",
  "https://your-production-frontend.com"
]
```

### Logging Configuration

In `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check PostgreSQL service is running
   - Verify connection string is correct
   - Ensure database user has appropriate permissions

2. **Migration Errors**
   - Run `dotnet ef migrations script` to see SQL that would be executed
   - Check if there are any pending migrations: `dotnet ef migrations list`

3. **JWT Token Issues**
   - Ensure secret key is sufficiently long (32+ characters recommended)
   - Check token expiration times in JwtConfig
   - Verify client is sending token in the Authorization header

4. **Performance Issues**
   - Check database indexes (see Developer Guide)
   - Monitor query performance using PostgreSQL's explain analyze
   - Consider enabling response compression for API

## Maintenance

### Database Backups

For PostgreSQL:
```bash
pg_dump -U sql_app -d sql_learning -F c -f backup.dump
```

To restore:
```bash
pg_restore -U sql_app -d sql_learning -c backup.dump
```

### Updates and Migrations

When updating the application:
1. Pull latest changes
2. Apply any new migrations: `dotnet ef database update`
3. Rebuild and restart the application

## Security Recommendations

1. Use HTTPS in production
2. Rotate JWT signing keys periodically
3. Set secure password policies
4. Implement rate limiting for API endpoints
5. Keep dependencies updated with security patches