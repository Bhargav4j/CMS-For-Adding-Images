using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.GalleryTypes;

public class DetailsModel : PageModel
{
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IGalleryTypeService galleryTypeService, ILogger<DetailsModel> logger)
    {
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

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
            _logger.LogError(ex, "Error loading gallery type details for ID {GalleryTypeId}", id);
            return NotFound();
        }
    }
}
