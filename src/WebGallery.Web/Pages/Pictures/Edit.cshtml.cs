using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class EditModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IWebHostEnvironment environment,
        ILogger<EditModel> logger)
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
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Gallery Type")]
        public int GalleryTypeId { get; set; }

        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }

        public string CurrentImagePath { get; set; } = string.Empty;
        public string CurrentThumbnailPath { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var picture = await _pictureService.GetByIdAsync(id.Value);
            if (picture == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = picture.Id,
                Name = picture.Name,
                Description = picture.Description,
                GalleryTypeId = picture.GalleryTypeId,
                CurrentImagePath = picture.ImagePath,
                CurrentThumbnailPath = picture.ThumbnailImagePath
            };

            await LoadGalleryTypesAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture for edit with ID {PictureId}", id);
            return NotFound();
        }
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
            var imagePath = Input.CurrentImagePath;
            var thumbnailPath = Input.CurrentThumbnailPath;

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

            var dto = new PictureUpdateDto
            {
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = imagePath,
                ThumbnailImagePath = thumbnailPath
            };

            await _pictureService.UpdateAsync(Input.Id, dto);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture with ID {PictureId}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the picture.");
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
