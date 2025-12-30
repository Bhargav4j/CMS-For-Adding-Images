using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using GalleryApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GalleryApp.Web.Pages.Admin.Pictures;

[Authorize(Roles = "admin")]
public class CreateModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IWebHostEnvironment environment,
        ILogger<CreateModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public PictureViewModel Picture { get; set; } = new PictureViewModel();

    public SelectList GalleryTypes { get; set; } = new SelectList(new List<GalleryTypeDto>(), "Id", "Name");

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create picture page");
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
            return Page();
        }

        try
        {
            if (Picture.ImageFile == null || Picture.ImageFile.Length == 0)
            {
                ModelState.AddModelError("Picture.ImageFile", "Please select an image file.");
                var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
                GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
                return Page();
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(Picture.ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("Picture.ImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
                GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
                return Page();
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            var thumbnailsFolder = Path.Combine(_environment.WebRootPath, "images", "thumbnails");

            Directory.CreateDirectory(uploadsFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            var thumbnailPath = Path.Combine(thumbnailsFolder, uniqueFileName);

            using (var image = await Image.LoadAsync(Picture.ImageFile.OpenReadStream()))
            {
                await image.SaveAsync(filePath);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(300, 225),
                    Mode = ResizeMode.Crop
                }));
                await image.SaveAsync(thumbnailPath);
            }

            var createDto = new PictureCreateDto
            {
                Name = Picture.Name,
                Description = Picture.Description,
                ImagePath = $"/images/{uniqueFileName}",
                ThumbnailImagePath = $"/images/thumbnails/{uniqueFileName}",
                GalleryTypeId = Picture.GalleryTypeId,
                CreatedBy = User.Identity?.Name ?? "admin"
            };

            await _pictureService.CreateAsync(createDto, cancellationToken);

            _logger.LogInformation("Picture created: {Name}", Picture.Name);

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the picture.");
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
            return Page();
        }
    }
}
