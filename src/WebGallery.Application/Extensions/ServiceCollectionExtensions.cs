using Microsoft.Extensions.DependencyInjection;
using WebGallery.Application.Services;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPictureService, PictureService>();
        services.AddScoped<IGalleryTypeService, GalleryTypeService>();

        return services;
    }
}
