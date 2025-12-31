using System.ComponentModel.DataAnnotations;
using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gallery.Web.Pages.Pictures;

[Authorize(Roles = "admin")]
public class CreateModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<CreateModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypeSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

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
        [StringLength(500)]
        [Display(Name = "Image Path")]
        public string ImagePath { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [Display(Name = "Thumbnail Path")]
        public string ThumbnailImagePath { get; set; } = string.Empty;
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
            var picture = new Picture
            {
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = Input.ImagePath,
                ThumbnailImagePath = Input.ThumbnailImagePath,
                CreatedBy = User.Identity?.Name ?? "system",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _pictureService.CreateAsync(picture);

            _logger.LogInformation("Picture created: {Name}", picture.Name);

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
        GalleryTypeSelectList = new SelectList(galleryTypes, "Id", "Name");
    }
}
