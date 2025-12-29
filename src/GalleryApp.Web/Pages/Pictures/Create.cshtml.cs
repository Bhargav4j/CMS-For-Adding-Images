using System.ComponentModel.DataAnnotations;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;
using GalleryApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GalleryApp.Web.Pages.Pictures;

[Authorize(Policy = "AdminOnly")]
public class CreateModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IPathService _pathService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IPathService pathService,
        ILogger<CreateModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _pathService = pathService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypes { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int GalleryTypeId { get; set; }

        [Required]
        public IFormFile? ImageFile { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadGalleryTypesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadGalleryTypesAsync();
            return Page();
        }

        try
        {
            if (Input.ImageFile == null || Input.ImageFile.Length == 0)
            {
                ModelState.AddModelError("Input.ImageFile", "Please select an image file");
                await LoadGalleryTypesAsync();
                return Page();
            }

            var uploadsFolder = Path.Combine(_pathService.GetWebRootPath(), "images", "uploads");
            var thumbnailsFolder = Path.Combine(_pathService.GetWebRootPath(), "images", "thumbnails");

            Directory.CreateDirectory(uploadsFolder);
            Directory.CreateDirectory(thumbnailsFolder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.ImageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            var thumbnailPath = Path.Combine(thumbnailsFolder, $"thumb_{fileName}");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Input.ImageFile.CopyToAsync(stream);
            }

            using (var image = await Image.LoadAsync(Input.ImageFile.OpenReadStream()))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Crop
                }));
                await image.SaveAsync(thumbnailPath);
            }

            var picture = new Picture
            {
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = $"/images/uploads/{fileName}",
                ThumbnailImagePath = $"/images/thumbnails/thumb_{fileName}",
                CreatedBy = User.Identity?.Name ?? "System",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _pictureService.CreateAsync(picture);

            _logger.LogInformation("Picture created successfully: {PictureName}", picture.Name);

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the picture");
            await LoadGalleryTypesAsync();
            return Page();
        }
    }

    private async Task LoadGalleryTypesAsync()
    {
        var galleryTypes = await _galleryTypeService.GetAllAsync();
        GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
    }
}
