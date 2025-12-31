using System.ComponentModel.DataAnnotations;
using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gallery.Web.Pages.Pictures;

[Authorize(Roles = "admin")]
public class EditModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<EditModel> logger)
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
        public int Id { get; set; }

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
                ImagePath = picture.ImagePath,
                ThumbnailImagePath = picture.ThumbnailImagePath
            };

            await LoadGalleryTypesAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture for edit: {Id}", id);
            return RedirectToPage("./Index");
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
            var picture = new Picture
            {
                Id = Input.Id,
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = Input.ImagePath,
                ThumbnailImagePath = Input.ThumbnailImagePath,
                ModifiedBy = User.Identity?.Name ?? "system",
                ModifiedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _pictureService.UpdateAsync(Input.Id, picture);

            _logger.LogInformation("Picture updated: {Id}", Input.Id);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture: {Id}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the picture.");
            await LoadGalleryTypesAsync();
            return Page();
        }
    }

    private async Task LoadGalleryTypesAsync()
    {
        var galleryTypes = await _galleryTypeService.GetAllAsync();
        GalleryTypeSelectList = new SelectList(galleryTypes, "Id", "Name", Input.GalleryTypeId);
    }
}
