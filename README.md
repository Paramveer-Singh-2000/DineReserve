[README.md](https://github.com/user-attachments/files/29836402/README.md)
# DineReserve

[![CI](https://github.com/Paramveer-Singh-2000/DineReserve/actions/workflows/ci.yml/badge.svg)](https://github.com/Paramveer-Singh-2000/DineReserve/actions/workflows/ci.yml)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![License: MIT](https://img.shields.io/badge/license-MIT-blue)

A restaurant & menu management API built with **.NET 10** and **Clean Architecture**, demonstrating CQRS with MediatR, policy- and claims-based authorization, and a deeply tested Application layer.

> Restaurant owners can list their restaurants and menus; customers can browse, search, sort, and paginate through them. Write operations are governed by ownership-based and role-based authorization policies.

## Why this project

This is a from-scratch implementation (not a tutorial clone) built to practice and demonstrate:
- **Clean Architecture** with strict dependency direction (Domain → Application → Infrastructure → API)
- **CQRS** using MediatR, with commands/queries/handlers/validators grouped by feature (vertical slices)
- **ASP.NET Core Identity** with a custom `ClaimsPrincipalFactory` and both role-based (`[Authorize(Roles = ...)]`) and requirement-based (`IAuthorizationHandler`) policies
- **FluentValidation** for request validation, decoupled from controllers
- **AutoMapper** profiles for entity ↔ DTO mapping
- **Structured logging** with Serilog (console + rolling file sinks)
- **A properly layered, deep test suite** — not just a token test project (see below)

## Tech stack

| Layer | Technologies |
|---|---|
| API | ASP.NET Core 10, Swagger/OpenAPI, Serilog.AspNetCore |
| Application | MediatR (CQRS), FluentValidation, AutoMapper |
| Domain | Plain C# entities, repository interfaces, domain exceptions |
| Infrastructure | EF Core 10 + SQL Server, ASP.NET Core Identity |
| CI | GitHub Actions (build + test on every push/PR) |

## Architecture

```
DineReserve.Domain            Entities, repository interfaces, domain exceptions, no external dependencies
   ↑
DineReserve.Application       CQRS commands/queries + handlers, validators, DTOs, AutoMapper profiles
   ↑
DineReserve.Infrastructure    EF Core DbContext, repository implementations, Identity, authorization handlers
   ↑
DineReserve.API               Controllers, middleware, DI wiring, Swagger
```

Each feature (e.g. `Restaurants/Commands/CreateRestaurant`) is a self-contained vertical slice: `Command`, `Validator`, and `Handler` live together instead of being split across technical folders.

### Authorization model

- **Read** and **Create** are open to any authenticated request (subject to role checks on the endpoint).
- **Update** and **Delete** on a restaurant (and its dishes) require the caller to be the restaurant's owner, *or* an Admin for Delete.
- A couple of endpoints also demonstrate claims-based policies (`RequireClaim`) and custom `IAuthorizationHandler` requirements (minimum age from a `DateOfBirth` claim, minimum restaurant count) — these exist to showcase the pattern, not as real product rules.

## Getting started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, a container, or a full instance)

### Run it

```bash
git clone https://github.com/Paramveer-Singh-2000/DineReserve.git
cd DineReserve

# point this at your own SQL Server instance if not using the default LocalDB config
# DineReserve.API/appsettings.json -> ConnectionStrings:DineReserveDb

dotnet restore
dotnet build
dotnet run --project DineReserve.API
```

On first run, the API applies the seed data (two sample restaurants with dishes, plus the `User`/`Admin`/`Owner` roles) and opens Swagger UI at `/swagger` in Development.


| Folder | What's covered |
|---|---|
| `Authorization/` | The ownership/role authorization service, and both custom `IAuthorizationHandler` requirements (minimum age, minimum restaurant count) — including edge cases like "turns 20 exactly today" and "no DOB claim set" |
| `Restaurants/` | Every Restaurant command/query handler, the `CreateRestaurant`/`UpdateRestaurant`/`GetAllRestaurant` validators, and `RestaurantRepositories` (CRUD, search-across-fields, sort direction, pagination) against a real EF Core InMemory database |
| `Dishes/` | Every Dish command/query handler, the `CreateDish` validator, and `DishesRepositories` |
| `Users/` | Role assignment/unassignment against mocked `UserManager`/`RoleManager`, profile updates, and `UserContext`'s claims-to-DTO mapping |
| `Middleware/` | `ErrorHandlingMiddleware` — asserts the actual HTTP status code and JSON body for each exception type, not just "it didn't throw" |
| `Common/` | `PagedResult`'s pagination math |

Handlers are tested with mocked dependencies (NSubstitute) so they run in milliseconds; repository tests run against EF Core's InMemory provider so the actual LINQ-to-EF query logic (search filters, sorting, skip/take) is verified rather than assumed. `UserManager<T>`/`RoleManager<T>` are mocked directly (their public members are virtual) rather than wrapped in extra abstraction layers just to make them testable.

A handful of these tests are deliberately regression tests, written to fail against an earlier, buggier version of the handler and pass against the current one — e.g. confirming that deleting a dish only ever touches the dish, or that page size is honored end-to-end through the repository.

## API overview

| Method | Route | Notes |
|---|---|---|
| GET | `/api/restaurant` | Search/sort/paginate restaurants (anonymous) |
| GET | `/api/restaurant/{id}` | Get one restaurant with its dishes |
| POST | `/api/restaurant` | Create a restaurant (`Owner` role) |
| PATCH | `/api/restaurant/{id}` | Update a restaurant (owner only) |
| DELETE | `/api/restaurant/{id}` | Delete a restaurant (owner or Admin) |
| GET | `/api/restaurants/{restaurantId}/dishes` | List dishes for a restaurant |
| GET | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Get a single dish |
| POST | `/api/restaurants/{restaurantId}/dishes` | Add a dish |
| DELETE | `/api/restaurants/{restaurantId}/dishes/{dishId}` | Delete a dish (owner or Admin) |
| PATCH | `/api/identity/user` | Update the current user's profile |
| POST/DELETE | `/api/identity` | Assign/unassign a role (Admin) |

Full request/response contracts are documented in Swagger when running in Development.

## Possible next steps

- Add integration tests with `WebApplicationFactory` against a real (containerized) SQL Server
- Add CORS configuration once a specific frontend origin is known
- Add rate limiting and API versioning
- Add a code coverage report to CI (coverlet is already wired into the test project)

## License

MIT
