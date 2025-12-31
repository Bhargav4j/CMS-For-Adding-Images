using Gallery.Domain.Interfaces.Repositories;
using Gallery.Infrastructure.Data;
using Gallery.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gallery.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services
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

        // Register repositories
        services.AddScoped<IPictureRepository, PictureRepository>();
        services.AddScoped<IGalleryTypeRepository, GalleryTypeRepository>();

        return services;
    }
}
