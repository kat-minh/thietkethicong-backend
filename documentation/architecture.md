# Architecture

Three-layer (N-tier) solution. Dependencies flow **inward**, one direction only:

```
Cms.API  ──►  Cms.Service  ──►  Cms.Repository
(HTTP,        (business         (EF Core,
 JWT,          logic,            PostgreSQL,
 CORS,         DTOs,             entities,
 Swagger)      auth, JWT)        migrations)
```

## Cms.Repository (data access)
- `Entities/` — Code-First entities (`User`).
- `Persistence/AppDbContext.cs` — EF Core context + model config.
- `Interfaces/` + `Repositories/` — repository abstraction over the context.
- `Migrations/` — committed migrations (applied automatically on startup).
- `Extensions/DependencyInjection.cs` — `AddPersistence(connectionString)`.

## Cms.Service (business logic)
- `DTOs/` — request/response contracts (`LoginRequest`, `LoginResponse`, ...).
- `Settings/JwtSettings.cs` — strongly-typed JWT config (bound from `Jwt` section).
- `Services/` — `AuthService` (login + profile), `TokenService` (JWT),
  `DatabaseSeeder` (default admin).
- `Extensions/DependencyInjection.cs` — `AddApplicationServices(configuration)`.

## Cms.API (presentation)
- `Controllers/AuthController.cs` — thin controllers delegating to services.
- `Program.cs` — wires layers, JWT auth, CORS, Swagger, and runs
  `Database.Migrate()` + seeding on startup with retry.

## Why this shape
- **Testable**: business logic in `Cms.Service` has no HTTP/EF dependency leaks.
- **Swappable data layer**: controllers depend on `IUserRepository`, not `DbContext`.
- **Matches the team convention** used in `shopaccgame-be`.
