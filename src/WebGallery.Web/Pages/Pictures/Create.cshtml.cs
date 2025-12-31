using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

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
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypes { get; set; } = new SelectList(new List<GalleryTypeDto>(), "Id", "Name");

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

    public async Task OnGetAsync()
    {
        await LoadGalleryTypesAsync();
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
            var imagePath = string.Empty;
            var thumbnailPath = string.Empty;

            if (Input.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(fileStream);
                }

                imagePath = $"/uploads/images/{uniqueFileName}";
                thumbnailPath = imagePath;
            }

            var dto = new PictureCreateDto
            {
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = imagePath,
                ThumbnailImagePath = thumbnailPath
            };

            await _pictureService.CreateAsync(dto);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the picture.");
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
