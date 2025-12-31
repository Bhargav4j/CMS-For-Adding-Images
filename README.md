# Web Gallery - .NET 8 Migration

This project has been successfully migrated from ASP.NET Web Forms 4.5 to .NET 8 with Clean Architecture.

## Project Structure

- **src/WebGallery.Domain**: Domain entities and interfaces
- **src/WebGallery.Application**: Business logic and services
- **src/WebGallery.Infrastructure**: Data access and EF Core
- **src/WebGallery.Web**: ASP.NET Core Razor Pages UI

## Technologies

- .NET 8
- ASP.NET Core Razor Pages
- Entity Framework Core 8.0
- ASP.NET Core Identity
- Serilog for logging
- SixLabors.ImageSharp for image processing
- Bootstrap 5 for UI

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server or LocalDB

### Running the Application

1. Update the connection string in `src/WebGallery.Web/appsettings.json`
2. Run the application:

```bash
cd src/WebGallery.Web
dotnet run
```

3. Default admin credentials:
   - Username: admin
   - Password: Admin@123

## Features

- Gallery browsing with categories
- User authentication and authorization
- Admin panel for managing pictures
- Image upload with automatic thumbnail generation
- Responsive design with Bootstrap 5

## Migration Notes

- Migrated from Web Forms to Razor Pages
- Replaced EF6 with EF Core 8
- Replaced Ninject with built-in DI
- Replaced Forms Authentication with ASP.NET Core Identity
- Migrated configuration from Web.config to appsettings.json
- Replaced System.Drawing with ImageSharp
