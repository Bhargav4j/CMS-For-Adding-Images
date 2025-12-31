using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.GalleryTypes;

public class DeleteModel : PageModel
{
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IGalleryTypeService galleryTypeService, ILogger<DeleteModel> logger)
    {
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    [BindProperty]
    public GalleryTypeDto? GalleryType { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            GalleryType = await _galleryTypeService.GetByIdAsync(id.Value);
            if (GalleryType == null)
            {
                return NotFound();
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery type for delete with ID {GalleryTypeId}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (GalleryType?.Id == null)
        {
            return NotFound();
        }

        try
        {
            await _galleryTypeService.DeleteAsync(GalleryType.Id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type with ID {GalleryTypeId}", GalleryType.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the gallery type.");
            return Page();
        }
    }
}
