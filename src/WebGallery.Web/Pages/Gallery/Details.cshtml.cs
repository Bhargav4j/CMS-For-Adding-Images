using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Gallery;

public class GalleryDetailsModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<GalleryDetailsModel> _logger;

    public GalleryDetailsModel(IPictureService pictureService, ILogger<GalleryDetailsModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public Picture? Picture { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Picture = await _pictureService.GetByIdAsync(id);

            if (Picture == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture details for ID: {Id}", id);
            return NotFound();
        }
    }
}
