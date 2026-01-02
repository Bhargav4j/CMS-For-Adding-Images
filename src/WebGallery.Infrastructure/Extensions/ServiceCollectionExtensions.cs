using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Infrastructure.Data;
using WebGallery.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebGallery.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


        services.AddScoped<IPictureRepository, PictureRepository>();
        services.AddScoped<IGalleryTypeRepository, GalleryTypeRepository>();

        return services;
    }
}
