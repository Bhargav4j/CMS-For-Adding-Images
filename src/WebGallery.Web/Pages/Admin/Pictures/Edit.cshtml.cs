using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Admin.Pictures;

[Authorize(Roles = "Admin")]
public class AdminPicturesEditModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<AdminPicturesEditModel> _logger;

    public AdminPicturesEditModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<AdminPicturesEditModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public Picture? Picture { get; set; }
    public SelectList GalleryTypeSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Gallery Type")]
        public int GalleryTypeId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Picture = await _pictureService.GetByIdAsync(id);

        if (Picture == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Name = Picture.Name,
            Description = Picture.Description,
            GalleryTypeId = Picture.GalleryTypeId
        };

        await LoadGalleryTypesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Picture = await _pictureService.GetByIdAsync(id);

        if (Picture == null)
        {
            return NotFound();
        }

        await LoadGalleryTypesAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            Picture.Name = Input.Name;
            Picture.Description = Input.Description;
            Picture.GalleryTypeId = Input.GalleryTypeId;
            Picture.ModifiedBy = User.Identity?.Name ?? "Admin";
            Picture.ModifiedDate = DateTime.UtcNow;

            await _pictureService.UpdateAsync(id, Picture);

            _logger.LogInformation("Picture updated: {Id}", id);

            return RedirectToPage("/Admin/Pictures/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture {Id}", id);
            ErrorMessage = "An error occurred while updating the picture.";
            return Page();
        }
    }

    private async Task LoadGalleryTypesAsync()
    {
        var galleryTypes = await _galleryTypeService.GetAllAsync();
        GalleryTypeSelectList = new SelectList(galleryTypes, "Id", "Name");
    }
}
