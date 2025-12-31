using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.ComponentModel.DataAnnotations;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Admin.Pictures;

[Authorize(Roles = "Admin")]
public class AdminPicturesCreateModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<AdminPicturesCreateModel> _logger;

    public AdminPicturesCreateModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IWebHostEnvironment environment,
        ILogger<AdminPicturesCreateModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypeSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a gallery type")]
        [Display(Name = "Gallery Type")]
        public int GalleryTypeId { get; set; }

        [Required(ErrorMessage = "Please select an image file")]
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadGalleryTypesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadGalleryTypesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.ImageFile == null || Input.ImageFile.Length == 0)
        {
            ErrorMessage = "Please select a valid image file.";
            return Page();
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(Input.ImageFile.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
            ErrorMessage = "Invalid file format. Only JPEG and PNG files are allowed.";
            return Page();
        }

        if (Input.ImageFile.Length > 10 * 1024 * 1024) // 10MB
        {
            ErrorMessage = "File size exceeds 10MB limit.";
            return Page();
        }

        try
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "gallery");
            var largeImagesFolder = Path.Combine(uploadsFolder, "largeImages");
            var thumbsFolder = Path.Combine(uploadsFolder, "thumbs");

            Directory.CreateDirectory(largeImagesFolder);
            Directory.CreateDirectory(thumbsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.ImageFile.FileName)}";
            var thumbFileName = $"thumb_{uniqueFileName}";

            var largeImagePath = Path.Combine(largeImagesFolder, uniqueFileName);
            var thumbPath = Path.Combine(thumbsFolder, thumbFileName);

            // Process and save the image
            using (var image = await Image.LoadAsync(Input.ImageFile.OpenReadStream()))
            {
                // Save original or resized large image
                if (image.Width > 1920 || image.Height > 1080)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(1920, 1080),
                        Mode = ResizeMode.Max
                    }));
                }

                await image.SaveAsJpegAsync(largeImagePath, new JpegEncoder { Quality = 90 });

                // Create thumbnail
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(300, 300),
                    Mode = ResizeMode.Max
                }));

                await image.SaveAsJpegAsync(thumbPath, new JpegEncoder { Quality = 85 });
            }

            // Create picture entity
            var picture = new Picture
            {
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = $"/images/gallery/largeImages/{uniqueFileName}",
                ThumbnailImagePath = $"/images/gallery/thumbs/{thumbFileName}",
                CreatedBy = User.Identity?.Name ?? "Admin",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _pictureService.CreateAsync(picture);

            SuccessMessage = "Picture uploaded successfully!";
            _logger.LogInformation("Picture created: {Name}", picture.Name);

            // Clear form
            Input = new InputModel();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading picture");
            ErrorMessage = "An error occurred while uploading the picture. Please try again.";
            return Page();
        }
    }

    private async Task LoadGalleryTypesAsync()
    {
        var galleryTypes = await _galleryTypeService.GetAllAsync();
        GalleryTypeSelectList = new SelectList(galleryTypes, "Id", "Name");
    }
}
