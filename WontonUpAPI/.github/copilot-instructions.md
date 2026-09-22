# Copilot instructions for WontonUpAPI

## Project overview

This repository contains a single ASP.NET Core Web API project targeting .NET 10:

- Solution: `WontonUpAPI.slnx`
- Project: `WontonUpAPI/WontonUpAPI.csproj`
- Namespace: `WontonUpAPI`
- Runtime data store: SQLite database in `WontonUpAPI/orders.db`

There is currently no README, contribution guide, test project, or configured linter in the repository.

## Build and run

Run these commands from the repository root:

```powershell
dotnet restore .\WontonUpAPI\WontonUpAPI.csproj
dotnet build .\WontonUpAPI.slnx
dotnet run --project .\WontonUpAPI\WontonUpAPI.csproj
```

The launch profiles expose:

- HTTP: `http://localhost:5267`
- HTTPS: `https://localhost:7074`
- Swagger/OpenAPI UI in Development at `/swagger`

The ready-to-use request examples are in `WontonUpAPI/WontonUpAPI.http`. The API can also be exercised with the OpenAPI document exposed in Development at `/openapi/v1.json`.

There is no test project or configured lint command at present. If tests are added, run the full suite with `dotnet test` and a single test with the appropriate filter, for example:

```powershell
dotnet test --filter "FullyQualifiedName~Namespace.TestClass.TestMethod"
```

## Architecture

`Program.cs` configures MVC controllers, OpenAPI, EF Core SQLite, and dependency injection. It registers `MenuService` as a singleton and `OrdersDbContext` through `AddDbContext`.

The API has two controller surfaces:

- `MenuController` serves `GET /menu` and optional `type` filtering.
- `OrdersController` serves `GET /orders`, `GET /orders/{id}`, and `POST /orders`.

Menu data is not stored in the database. `MenuService` loads `Services/Menu.json` once at startup into memory and provides lookup/filter operations. Keep the JSON file path and the singleton lifetime in mind when changing menu behavior.

Orders are persisted through `OrdersDbContext` to SQLite using the `DefaultConnection` connection string from configuration. Startup calls `EnsureCreated()`; this project does not currently use EF Core migrations. The development database is therefore created automatically from the model, and the local `orders.db` plus SQLite sidecar files are runtime artifacts.

When an order is created, `OrdersController`:

1. Validates that the request contains at least one menu item ID.
2. Resolves every ID through `MenuService`.
3. Groups duplicate IDs into quantities and calculates each line total and the order total.
4. Serializes the calculated ordered-item snapshot into `Order.ItemsJson`.
5. Stores UTC creation time, a 15-minute ETA, and state `"waiting"`.
6. Returns a receipt DTO with `201 Created` and a route to `GET /orders/{id}`.

Reading an order deserializes `ItemsJson` and maps the database entity to an `OrderDto`. Ordered item details are intentionally snapshotted in the order row, so later changes to `Menu.json` do not rewrite historical order contents.

## Code conventions

- Keep API route templates lowercase (`menu`, `orders`) and use attribute-routed controllers.
- Controllers return `IActionResult` and use `Ok`, `CreatedAtAction`, `BadRequest`, and `NotFound` for HTTP responses.
- Invalid order requests use the existing anonymous error shape:
  `{ "error": "...", "message": "..." }`.
- Keep transport types in `DTOs/`; persistence/domain types belong in `Models/`.
- `Order` stores ordered items as serialized JSON rather than a relational child table. Preserve the `ItemsJson` serialization/deserialization contract when changing order responses.
- `OrderedItemDto.Price` and receipt item `Price` represent the total for that line (`unit price * quantity`), not the unit price.
- Use `decimal` for menu prices and order totals.
- New order timestamps should remain UTC. Existing order responses format timestamps as round-trip strings (`"o"`); the create receipt currently uses `DateTime.ToString()` and should not be changed casually because it affects the response contract.
- Preserve the current default order state value, `"waiting"`, unless the API contract is being intentionally changed.
- `CreateOrderDto.Items` is an integer array and uses data annotations for required/non-empty validation; controller logic also explicitly guards null/empty input and invalid menu IDs.
- `MenuService` returns all items when no type is supplied and performs exact, case-sensitive type matching.
- Keep OpenAPI/Swagger setup inside the Development environment guard.
- Configuration belongs in `appsettings.json` or environment-specific configuration; the default database connection is `Data Source=orders.db`.
