using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Infrastructure.Data;
using GalleryApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GalleryApp.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering Infrastructure layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<GalleryDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("GalleryDB"),
                b => b.MigrationsAssembly(typeof(GalleryDbContext).Assembly.FullName)));

        // Register Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<GalleryDbContext>()
        .AddDefaultTokenProviders();

        // Register repositories
        services.AddScoped<IPictureRepository, PictureRepository>();
        services.AddScoped<IGalleryTypeRepository, GalleryTypeRepository>();

        return services;
    }
}
