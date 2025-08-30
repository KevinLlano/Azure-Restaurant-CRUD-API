# RestaurantAPI (.NET 9 + EF Core + SQL Server)

A simple REST API for a restaurant domain: Customers, FoodItems, Orders (OrderMasters/OrderDetails). Uses EF Core code?first with migrations and SQL Server.

## Tech stack
- .NET 9 SDK
- ASP.NET Core Web API
- EF Core (SqlServer, Design)
- SQL Server (Express or LocalDB)
- Swagger UI

## Prerequisites
- .NET 9 SDK installed (`dotnet --version`)
- SQL Server running locally (SQLEXPRESS or LocalDB)
- Git
- EF Core tools: `dotnet tool install --global dotnet-ef`

## Repository layout
- Solution root: `C:/Projects/RestaurantAPI`
- Project: `RestaurantAPI/RestaurantAPI.csproj`
- DbContext: `Models/RestaurantDbContext.cs`
- Entities: `Customer`, `FoodItem`, `OrderMaster`, `OrderDetail`

## Configuration
Edit `RestaurantAPI/appsettings.json`:
- ConnectionStrings.DevConnection (example for SQL Express):
  - `Server=localhost\SQLEXPRESS;Database=RestaurantDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;`
- Example for LocalDB:
  - `Server=(localdb)\MSSQLLocalDB;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=True;`

`Program.cs` is configured to apply migrations automatically on startup in development:
- `context.Database.Migrate();`

> Do not mix `EnsureCreated()` with migrations on the same database.

## Setup (first time)
From the project directory (`RestaurantAPI`):

1) Restore and build
- `dotnet restore`
- `dotnet build`

2) Install packages/tools (if not already installed)
- `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
- `dotnet add package Microsoft.EntityFrameworkCore.Design`
- `dotnet tool install --global dotnet-ef`

3) Create the database schema
- If migrations already exist (folder `Migrations` present):
  - `dotnet ef database update --context RestaurantDbContext`
- If starting fresh (no migrations):
  - `dotnet ef migrations add InitialCreate --context RestaurantDbContext`
  - `dotnet ef database update --context RestaurantDbContext`

## Run
- `dotnet run`
- Swagger UI: open the URL printed in console (e.g., `http://localhost:5000/swagger`).

## API quickstart
- Customers
  - `GET /api/Customer`
- Food items
  - `GET /api/FoodItem`
  - `GET /api/FoodItem/{id}`
  - `POST /api/FoodItem`
  - `PUT /api/FoodItem/{id}`
  - `DELETE /api/FoodItem/{id}`
- Orders
  - Explore in Swagger if `OrderController` is present.

## Development workflow
- Change models (e.g., add fields).
- Add a migration:
  - `dotnet ef migrations add AddSomeChange --context RestaurantDbContext`
- Update the database:
  - `dotnet ef database update --context RestaurantDbContext`

## Reset database (dev only)
If you previously used `EnsureCreated()` or want a clean slate:
- `dotnet ef database drop --force --context RestaurantDbContext`
- `dotnet ef database update --context RestaurantDbContext`

## Git Bash in Visual Studio (optional)
- Tools > Options > Environment > Terminal > Add:
  - Name: `GitBash`
  - Shell Location (preferred): `C:\\Program Files\\Git\\bin\\bash.exe`
    - Alternative: `C:\\Program Files\\Git\\git-bash.exe`
  - Arguments: `--login -i`

Using Git Bash:
- `cd "/c/Projects/RestaurantAPI/RestaurantAPI" && dotnet ef database update && dotnet run`

## Troubleshooting
- "dotnet-ef not found":
  - `dotnet tool install --global dotnet-ef` and restart terminal.
- "Invalid object name 'FoodItems'":
  - Ensure migrations are applied: `dotnet ef database update`.
  - Verify `ConnectionStrings:DevConnection` points to the expected SQL instance.
- Duplicate FK/CustomerID errors during migration:
  - Ensure this mapping exists in `OnModelCreating`:
    - `modelBuilder.Entity<OrderMaster>()
        .HasOne(om => om.Customer)
        .WithMany(c => c.Orders)
        .HasForeignKey(om => om.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);`
  - If migrations are inconsistent, remove bad ones and recreate:
    - `dotnet ef migrations remove` (repeat until clean) or delete the `Migrations` folder (dev only), then add `InitialCreate` again.

## Notes
- Project targets C# 13 / .NET 9.
- Non-nullable strings in entities should be provided values when creating records (or mark them `required`/provide defaults).
