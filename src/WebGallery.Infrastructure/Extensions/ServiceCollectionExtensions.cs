using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Infrastructure.Data;
using WebGallery.Infrastructure.Repositories;

namespace WebGallery.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering Infrastructure layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        services.AddScoped<IPictureRepository, PictureRepository>();
        services.AddScoped<IGalleryTypeRepository, GalleryTypeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
