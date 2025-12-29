# Gallery Application - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.5 to .NET 8 using clean architecture principles.

## Project Structure

```
GalleryApp/
├── src/
│   ├── GalleryApp.Domain/          # Domain entities and interfaces
│   ├── GalleryApp.Application/     # Business logic and services
│   ├── GalleryApp.Infrastructure/  # Data access and EF Core
│   └── GalleryApp.Web/             # Razor Pages UI
└── GalleryApp.sln
```

## Prerequisites

- .NET 8 SDK
- SQL Server or LocalDB
- Visual Studio 2022 or VS Code

## Getting Started

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Update connection string:**
   Edit `src/GalleryApp.Web/appsettings.json` and update the connection string if needed.

3. **Run migrations:**
   ```bash
   dotnet ef database update --project src/GalleryApp.Infrastructure --startup-project src/GalleryApp.Web
   ```

4. **Run the application:**
   ```bash
   dotnet run --project src/GalleryApp.Web
   ```

5. **Default credentials:**
   - Username: `admin`
   - Password: `Admin@123`

## Features

- User authentication with ASP.NET Core Identity
- Picture gallery management (CRUD operations)
- Gallery type categorization
- Image upload with thumbnail generation
- Role-based authorization (admin/user)
- Responsive Bootstrap 5 UI

## Technologies

- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8
- ASP.NET Core Identity
- SQL Server
- Serilog for logging
- SixLabors.ImageSharp for image processing
- Bootstrap 5

## Migration Notes

This application was migrated from ASP.NET Web Forms to .NET 8:

- Web Forms pages → Razor Pages
- System.Web → ASP.NET Core equivalents
- Entity Framework 6 → EF Core 8
- Forms Authentication → ASP.NET Core Identity
- Web.config → appsettings.json
- Global.asax → Program.cs
- HTTP Handlers/Modules → Middleware/Endpoints

## Build Status

✅ Build succeeded with 0 errors and 6 warnings (nullable reference type warnings only)
