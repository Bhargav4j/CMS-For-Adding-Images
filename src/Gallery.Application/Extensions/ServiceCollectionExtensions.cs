using Gallery.Application.Services;
using Gallery.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gallery.Application.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPictureService, PictureService>();
        services.AddScoped<IGalleryTypeService, GalleryTypeService>();

        return services;
    }
}
