# Modular Monolith .NET Template

Clean Architecture, organized by **Module** rather than applying horizontal layers across the entire application.
Each module acts as a self-contained "mini Clean Architecture" (Domain → Application → Infrastructure), and the Host simply glues them together.

## Project Structure

```text
src/
  BuildingBlocks/             # Shared across ALL modules — strictly NO business logic
    BuildingBlocks.Domain/        BaseEntity, generic IRepository<T>, generic IUnitOfWork
    BuildingBlocks.Application/   PaginatedList, shared exceptions, IDateTimeProvider
    BuildingBlocks.Infrastructure/ RepositoryBase<T> (EF Core generic implementation)

  Modules/
    Users/                    # <- Example module; clone this structure for new modules
      Users.Domain/
        Entities/User.cs
        Interfaces/           # IUserRepository, IUsersUnitOfWork, IJwtTokenService
                              # (Business-facing interfaces -> implemented in Infrastructure)
      Users.Application/
        DTOs/
        Services/AuthService.cs
        Interfaces/IEmailSender.cs   # Non-business interface -> implemented DIRECTLY in Application
                                     # (Services/NoOpEmailSender.cs), NOT placed in Infrastructure
      Users.Infrastructure/
        Persistence/UsersDbContext.cs, UserRepository.cs, UsersUnitOfWork.cs
        Auth/JwtTokenService.cs
        DependencyInjection.cs  # AddUsersModule() — Host calls this single line

  Host/
    WebApi/                   # Program.cs composes modules, configures JWT + Swagger
      Controllers/AuthController.cs  # Lean controller, delegates logic to Application Services

tests/
  Users.Tests/                # Unit test Application Services by mocking the UnitOfWork

```

## Architectural Conventions

* **Business-facing Interfaces & Repositories** (e.g., `IJwtTokenService`, as token generation is tightly bound to domain rules like `User` roles) → Declare in `{Module}.Domain/Interfaces` and implement in `{Module}.Infrastructure`.
* **Non-business Technical Interfaces** (e.g., Email senders, DateTime providers, File Storage) → Declare in `{Module}.Application/Interfaces` and **implement directly inside Application**, avoiding Infrastructure overhead when third-party SDK dependencies aren't required.
* **Isolated Database Schemas per Module** → Each module owns a dedicated `DbContext` and schema (`modelBuilder.HasDefaultSchema("users")`). **Cross-DbContext querying is strictly prohibited** — fetch data from other modules strictly via their public Application services or APIs.
* **Encapsulated Dependency Injection** → Every module exposes a single `DependencyInjection.cs` file with an extension method `Add{Module}Module(configuration)`. The Host (`Program.cs`) invokes this single entry point with zero knowledge of the module's internal wiring.

## Adding a New Module (e.g., Payroll)

1. **Clone & Rename:** Copy the three projects (`Users.Domain`, `Users.Application`, `Users.Infrastructure`), rename them to `Payroll.*`, and update namespaces accordingly.
2. **Domain:** Define Entities + `IPayrollRepository` + `IPayrollUnitOfWork` (extending `IUnitOfWork`).
3. **Application:** Create DTOs and Application Services targeting `IPayrollUnitOfWork`.
4. **Infrastructure:** Implement `PayrollDbContext` (isolated schema `"payroll"`), repositories, Unit of Work, and the `AddPayrollModule()` DI extension method.
5. **Host Integration:** Add `builder.Services.AddPayrollModule(builder.Configuration);` inside `Program.cs`.
6. **Solution Wiring:** Add new projects to `.sln` and expose API endpoints via a new controller in `Host/WebApi/Controllers`.

## Getting Started

```bash
dotnet restore
dotnet ef migrations add InitialUsers -p src/Modules/Users/Users.Infrastructure -s src/Host/WebApi -c UsersDbContext
dotnet ef database update -p src/Modules/Users/Users.Infrastructure -s src/Host/WebApi -c UsersDbContext
dotnet run --project src/Host/WebApi

```

> **Note:** Remember to update `Jwt:SigningKey` in `appsettings.json` prior to deployment — the default key is intended strictly for local development.

## Intentionally Omitted from Template

* **MediatR / CQRS:** The current pattern utilizes direct Service classes invoking the Unit of Work. If CQRS is required, implement it inside the Application layer of each specific module without affecting Domain or Infrastructure layers.
* **Pre-generated Database Migrations:** Run the migration commands listed in the "Getting Started" section after downloading the template.
* **Production Extensions:** Features such as Refresh Token rotation, actual SMTP integrations, and rate-limiting are left to be configured based on project-specific demands.