## NetCore-API-NLayer-Architecture

Production‑oriented .NET 8 Web API structured with a clean N‑Layer architecture. The solution separates API, Service, and Repository concerns for maintainability, testability, and scalability.

### Table of Contents
- **Overview**
- **Architecture**
- **Tech Stack**
- **Setup**
- **Configuration**
- **Migrations & Database**
- **Run**
- **Useful Commands**
---

### Overview
This repository contains an ASP.NET Core Web API that uses Entity Framework Core for data access and SQL Server as the database. Swagger is enabled for interactive API documentation. The solution is organized into API, Services, and Repositories projects to keep responsibilities clearly separated.

Solution layout: `NetCore-API-NLayer-Architecture/`
- `App.API` (Presentation – Web API)
- `Services` (Business/Domain layer)
- `Repositories` (EF Core, DbContext, Migrations)

---

### Architecture
- **App.API**
  - Minimal hosting via `Program.cs`, controllers for endpoints
  - Swagger/OpenAPI enabled
  - DI registration: `builder.Services.AddRepositories(builder.Configuration);`
- **Services**
  - Place for application/business logic (currently contains `ServiceAssembly` marker)
- **Repositories**
  - EF Core `AppDbContext`
  - `Extensions/RepositoryExtensions.cs` registers `DbContext`
  - Uses `ApplyConfigurationsFromAssembly(...)` to load entity configurations
  - Migrations live in this project; startup app is `App.API`

Flow: `App.API` → `Services` → `Repositories` → SQL Server

---

### Tech Stack
- .NET 8 (C# 12)
- ASP.NET Core Web API
- Entity Framework Core 8 (SqlServer)
- Swagger (Swashbuckle)
- Built‑in Microsoft Dependency Injection

---

### Setup
1) Requirements
- .NET SDK 8.x
- SQL Server (LocalDB or full instance)
- (Optional) Git, PowerShell / Terminal

2) Clone the repository
```bash
git clone <REPO_URL>
cd NetCore-API-NLayer-Architecture
```

3) (Optional) Install EF CLI
```bash
dotnet tool install --global dotnet-ef
```

---

### Configuration
Connection string is located in `App.API/appsettings.Development.json`. Update `ConnectionStrings:SqlServer` for your environment.
```json
{
  "ConnectionStrings": {
    "SqlServer": "Data Source=.;Integrated Security=True;Initial Catalog=NLayerCleanArchDb;Encrypt=True;Trust Server Certificate=True"
  }
}
```
The `Repositories/ConnectionStringsOption.cs` class exposes the configuration key `"ConnectionStrings"`, which is consumed by `RepositoryExtensions`.

---

### Migrations & Database
Create and apply migrations to the `Repositories` project, using `App.API` as the startup project.

```bash
# From solution root
cd NetCore-API-NLayer-Architecture/App.API

# First migration
dotnet ef migrations add InitialCreate -p ../Repositories/App.Repositories.csproj -s App.API.csproj

# Apply to database
dotnet ef database update -p ../Repositories/App.Repositories.csproj -s App.API.csproj
```

Note: Ensure `App.API` is not running while executing EF commands.

---

### Run
```bash
# Run from API project
cd NetCore-API-NLayer-Architecture/App.API
dotnet run
```
- Swagger UI: `https://localhost:<port>/swagger`
- Sample endpoint: `Categories`

Extend with your own services and controllers (e.g., Products CRUD) as needed.

---

### Useful Commands
```bash
# Clean and build
dotnet clean && dotnet build

# All tests (if you add some)
dotnet test

# EF Core help
dotnet ef --help
```

---

