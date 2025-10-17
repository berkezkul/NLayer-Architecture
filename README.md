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
- **Real‑time & Messaging (SignalR / WebSocket / RabbitMQ)**
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
- Sample endpoint: `Notifications` (POST `/Notifications/broadcast`)

Extend with your own services and controllers (e.g., Products CRUD) as needed.

---

### Real‑time & Messaging (SignalR / WebSocket / RabbitMQ)

This solution includes real‑time updates to clients via SignalR (built on WebSockets) and optional asynchronous messaging via RabbitMQ (MassTransit).

- Why SignalR?
  - Bi‑directional, low‑latency updates over WebSockets with automatic fallback and reconnection handling.
  - Simplifies broadcasting to all clients or groups without managing raw socket plumbing.
  - Impact: Users see immediate UI updates (e.g., new records, progress, notifications) without polling.

- Why RabbitMQ (MassTransit)?
  - Decoupled, reliable, and scalable background processing via message queues.
  - Enables eventual consistency and resilience for long‑running or high‑throughput workflows.
  - Impact: Protects API responsiveness; work can be retried and scaled horizontally.

Included server components:
- Hub: `/hubs/notifications` (see `App.API/Hubs/NotificationsHub.cs`).
- Broadcast API: `POST /Notifications/broadcast` sends a `broadcast` message to all connected clients.
- MassTransit configuration with RabbitMQ host in `appsettings.Development.json` (`RabbitMQ` section).

Quick test (client):
```javascript
// npm i @microsoft/signalr
import { HubConnectionBuilder } from '@microsoft/signalr';

const connection = new HubConnectionBuilder()
  .withUrl('https://localhost:<port>/hubs/notifications')
  .build();

connection.on('broadcast', data => console.log('broadcast:', data));
await connection.start();
```

Then call from Swagger:
```json
POST /Notifications/broadcast
{ "message": "Hello from server" }
```

RabbitMQ (optional):
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
# UI: http://localhost:15672 (guest/guest)
```
MassTransit is registered in `Program.cs`. Add consumers in a worker or API project as needed.

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

