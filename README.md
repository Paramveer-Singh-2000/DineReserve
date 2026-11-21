# DineReserve — restaurant management API

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)

**An ASP.NET Core REST API demonstrating Clean Architecture, CQRS with MediatR, and EF Core / SQL Server.**

DineReserve manages restaurants, dishes, user roles, and authorized write operations. The repository separates Domain, Application, Infrastructure, and API concerns and includes a substantial xUnit test project.

## Technology

- **API:** ASP.NET Core 10, controllers, Swagger/OpenAPI
- **Application:** MediatR, FluentValidation, AutoMapper
- **Infrastructure:** EF Core 10, SQL Server, ASP.NET Core Identity
- **Testing:** xUnit and mocked application dependencies; EF Core InMemory tests
- **Other:** Role-based, claims-based, and resource-ownership authorization

## Architecture

```text
DineReserve.API             Controllers, HTTP pipeline, Swagger, auth
         |
DineReserve.Application     Commands, queries, handlers, DTOs, validators
         |
DineReserve.Domain          Entities, contracts, exceptions
         ^
DineReserve.Infrastructure  Persistence, repository implementations, authorization
```

Projects are organized around a layered architecture. The Application layer groups commands, handlers, and validators by feature.

## Run locally

**Prerequisites:** .NET 10 SDK and SQL Server.

```bash
git clone https://github.com/Paramveer-Singh-2000/DineReserve.git
cd DineReserve
dotnet restore DineReserve.slnx
dotnet build DineReserve.slnx
```

Configure `ConnectionStrings:DineReserveDb` for your SQL Server instance, preferably using local .NET user secrets or environment variables. Use EF Core migrations to set up the database before running the app. Note: the current API startup code does **not** automatically apply migrations or call the seeder.

```bash
dotnet ef database update --project DineReserve.Infrastructure --startup-project DineReserve.API
dotnet run --project DineReserve.API
```

Find the Development-mode Swagger UI at `/swagger` on the host/port printed by `dotnet run`.

## Selected endpoints

| Method | Route | Purpose / access |
| --- | --- | --- |
| GET | `/api/restaurant` | List/search restaurants; anonymous |
| GET | `/api/restaurant/{id}` | Get restaurant; claim-based policy |
| POST | `/api/restaurant` | Create restaurant; Owner role |
| PATCH | `/api/restaurant/{id}` | Update restaurant; authenticated, ownership checked in application logic |
| DELETE | `/api/restaurant/{id}` | Delete restaurant; authenticated, ownership checked in application logic |
| GET | `/api/restaurants/{restaurantId}/dishes` | List dishes; minimum-age policy |
| GET | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Retrieve a dish; authenticated |
| POST | `/api/restaurants/{restaurantId}/dishes` | Add a dish; authenticated |
| DELETE | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Delete a dish; authenticated |
| PATCH | `/api/identity/user` | Update user profile; authenticated |

ASP.NET Core Identity endpoints are also mapped under `/api/identity`; the controller provides Admin-only role assignment/unassignment operations.

## Testing

```bash
dotnet test DineReserve.slnx
```

The test project includes tests for restaurant and dish handlers, validators, repository query behavior, authorization requirements, user context, and middleware.

## Repository layout

```text
DineReserve.API/
DineReserve.Application/
DineReserve.Domain/
DineReserve.Infrastructure/
DineReserve.Application.Tests/
```

## Development notes

The project remains a portfolio implementation. Before claiming production readiness, validate fresh-install setup, review authentication and authorization paths, ensure build/test CI is operational, audit package vulnerabilities, and add integration tests. No LICENSE file or active GitHub Actions workflow was found during the repository review, so neither is advertised here.

## Author

**Paramveer Singh** · [GitHub](https://github.com/Paramveer-Singh-2000) · [LinkedIn](https://www.linkedin.com/in/paramveer-singh-8555b1184/)
