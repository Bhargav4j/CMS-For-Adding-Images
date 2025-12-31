using Microsoft.Extensions.DependencyInjection;
using WebGallery.Application.Mappings;
using WebGallery.Application.Services;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddScoped<IPictureService, PictureService>();
        services.AddScoped<IGalleryTypeService, GalleryTypeService>();

        return services;
    }
}
