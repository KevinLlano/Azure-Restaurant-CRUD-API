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
  - `dotton ef database update --context RestaurantDbContext`

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

## Build this project from scratch (Visual Studio GUI)
These steps work for either the "ASP.NET Core Web API" template or the "ASP.NET Core Web App (Model-View-Controller)" template. This project uses API controllers; the Web API template is recommended.

1) Create the solution and project
- File > New > Project > ASP.NET Core Web API
- Framework: .NET 9
- Enable Use controllers; Enable OpenAPI support

2) Add EF Core packages
- Project > Manage NuGet Packages
  - Install `Microsoft.EntityFrameworkCore.SqlServer`
  - Install `Microsoft.EntityFrameworkCore.Design`

3) Add the connection string
- In `appsettings.json` add:
```
"ConnectionStrings": {
  "DevConnection": "Server=localhost\\SQLEXPRESS;Database=RestaurantDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
}
```

4) Create entity classes (Models folder)
- Customer
```
public class Customer {
  [Key] public int CustomerID { get; set; }
  [Column(TypeName = "nvarchar(100)")][Required]
  public string CustomerName { get; set; } = string.Empty;
  public virtual ICollection<OrderMaster> Orders { get; set; } = new List<OrderMaster>();
}
```
- FoodItem
```
public class FoodItem {
  [Key] public int FoodItemId { get; set; }
  [Column(TypeName = "nvarchar(100)")] public string FoodItemName { get; set; } = string.Empty;
  public decimal Price { get; set; }
}
```
- OrderMaster
```
public class OrderMaster {
  [Key] public long OrderMasterId { get; set; }
  [Column(TypeName = "nvarchar(75)")] public string OrderNumber { get; set; } = string.Empty;
  public int CustomerId { get; set; }
  public Customer Customer { get; set; } = null!;
  [Column(TypeName = "nvarchar(10)")] public string PMethod { get; set; } = string.Empty;
  public decimal GTotal { get; set; }
  public List<OrderDetail> OrderDetails { get; set; } = new();
  [NotMapped] public string DeletedOrderItemIds { get; set; } = string.Empty;
}
```
- OrderDetail
```
public class OrderDetail {
  [Key] public long OrderDetailId { get; set; }
  public long OrderMasterId { get; set; }
  public int FoodItemId { get; set; }
  public FoodItem FoodItem { get; set; } = null!;
  public decimal FoodItemPrice { get; set; }
  public int Quantity { get; set; }
}
```

5) Create the DbContext (Models/RestaurantDbContext.cs)
```
public class RestaurantDbContext : DbContext {
  public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) {}
  public DbSet<Customer> Customers => Set<Customer>();
  public DbSet<FoodItem> FoodItems => Set<FoodItem>();
  public DbSet<OrderMaster> OrderMasters => Set<OrderMaster>();
  public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<OrderMaster>()
      .HasOne(om => om.Customer)
      .WithMany(c => c.Orders)
      .HasForeignKey(om => om.CustomerId)
      .OnDelete(DeleteBehavior.Restrict);
    modelBuilder.Entity<OrderDetail>()
      .HasOne(od => od.FoodItem)
      .WithMany()
      .HasForeignKey(od => od.FoodItemId)
      .OnDelete(DeleteBehavior.Restrict);
    modelBuilder.Entity<OrderDetail>()
      .HasOne<OrderMaster>()
      .WithMany(om => om.OrderDetails)
      .HasForeignKey(od => od.OrderMasterId)
      .OnDelete(DeleteBehavior.Cascade);
    modelBuilder.Entity<FoodItem>().Property(f => f.Price).HasColumnType("decimal(18,2)");
    modelBuilder.Entity<OrderMaster>().Property(om => om.GTotal).HasColumnType("decimal(18,2)");
    modelBuilder.Entity<OrderDetail>().Property(od => od.FoodItemPrice).HasColumnType("decimal(18,2)");
  }
}
```

6) Register DbContext (Program.cs)
```
builder.Services.AddDbContext<RestaurantDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// after app = builder.Build();
using (var scope = app.Services.CreateScope()) {
  var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
  db.Database.Migrate();
}
```

7) Add controllers (Controllers folder)
- FoodItemController (basic CRUD)
- CustomerController (GET list)

8) Add and apply migrations
- Tools > NuGet Package Manager > Package Manager Console
  - `Add-Migration InitialCreate -Context RestaurantDbContext`
  - `Update-Database -Context RestaurantDbContext`

9) Run and test
- Press F5 or `dotnet run`
- Open Swagger at the printed URL and exercise endpoints

## Build this project from scratch (CLI)
1) Create the solution and project
- `dotnet new sln -n RestaurantAPI`
- `mkdir RestaurantAPI && cd RestaurantAPI`
- `dotnet new webapi -n RestaurantAPI`
- `dotnet sln .. add RestaurantAPI/RestaurantAPI.csproj`

2) Add packages
- `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
- `dotnet add package Microsoft.EntityFrameworkCore.Design`

3) Add connection string in `appsettings.json` (see above)

4) Create Models and DbContext as shown above

5) Register DbContext in `Program.cs` and call `Database.Migrate()` as shown above

6) Create and apply migrations
- `dotnet tool install --global dotnet-ef`
- `dotnet ef migrations add InitialCreate --context RestaurantDbContext`
- `dotnet ef database update --context RestaurantDbContext`

7) Run
- `dotnet run`

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
