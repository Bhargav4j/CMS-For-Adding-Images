using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

[Authorize(Roles = "admin")]
public class PictureCreateModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PictureCreateModel> _logger;

    public PictureCreateModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IWebHostEnvironment environment,
        ILogger<PictureCreateModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypes { get; set; } = new SelectList(new List<GalleryType>(), "Id", "Name");

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Gallery Type")]
        public int GalleryTypeId { get; set; }

        [Required]
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
        GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
            return Page();
        }

        try
        {
            var uploadBasePath = Environment.GetEnvironmentVariable("UPLOAD_PATH") ?? Path.Combine(_environment.WebRootPath, "images", "gallery");
            var uploadsPath = uploadBasePath;
            var thumbsPath = Path.Combine(uploadsPath, "thumbs");
            Directory.CreateDirectory(uploadsPath);
            Directory.CreateDirectory(thumbsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(Input.ImageFile!.FileName)}";
            var imagePath = Path.Combine(uploadsPath, fileName);
            var thumbnailPath = Path.Combine(thumbsPath, fileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await Input.ImageFile.CopyToAsync(stream, cancellationToken);
            }

            using (var image = await Image.LoadAsync(imagePath, cancellationToken))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                }));
                await image.SaveAsync(thumbnailPath, cancellationToken);
            }

            var picture = new Picture
            {
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = $"/images/gallery/{fileName}",
                ThumbnailImagePath = $"/images/gallery/thumbs/{fileName}",
                CreatedBy = User.Identity?.Name ?? "Unknown",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _pictureService.CreateAsync(picture, cancellationToken);

            _logger.LogInformation("Picture created: {PictureName}", Input.Name);

            return RedirectToPage("./Index");
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
