using GalleryApp.Application.Services;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GalleryApp.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        // Register services
        services.AddScoped<IPictureService, PictureService>();
        services.AddScoped<IGalleryTypeService, GalleryTypeService>();

        return services;
    }
}
