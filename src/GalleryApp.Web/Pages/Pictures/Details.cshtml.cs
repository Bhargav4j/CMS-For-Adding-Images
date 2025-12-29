using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Pictures;

[Authorize(Policy = "AdminOnly")]
public class DetailsModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IPictureService pictureService, ILogger<DetailsModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public Picture? Picture { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            Picture = await _pictureService.GetByIdAsync(id.Value);

            if (Picture == null)
            {
                _logger.LogWarning("Picture not found with ID: {PictureId}", id.Value);
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving picture details for ID: {PictureId}", id.Value);
            return RedirectToPage("Index");
        }
    }
}
