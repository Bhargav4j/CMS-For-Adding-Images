using GalleryApp.Application.Services;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Domain.Interfaces.Services;
using GalleryApp.Infrastructure.Data;
using GalleryApp.Infrastructure.Repositories;
using GalleryApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/galleryapp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GalleryDB")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IPictureRepository, PictureRepository>();
builder.Services.AddScoped<IGalleryTypeRepository, GalleryTypeRepository>();
builder.Services.AddScoped<IPictureService, PictureService>();
builder.Services.AddScoped<IGalleryTypeService, GalleryTypeService>();
builder.Services.AddScoped<IPathService>(provider =>
{
    var env = provider.GetRequiredService<IWebHostEnvironment>();
    return new PathService(env.WebRootPath, env.ContentRootPath);
});

builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/intro", () => Results.Redirect("/Gallery?type=intro"));
app.MapGet("/книги", () => Results.Redirect("/Gallery?type=books"));
app.MapGet("/проза", () => Results.Redirect("/Gallery?type=prose"));
app.MapGet("/тортики", () => Results.Redirect("/Gallery?type=cakes"));

try
{
    Log.Information("Starting GalleryApp web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
