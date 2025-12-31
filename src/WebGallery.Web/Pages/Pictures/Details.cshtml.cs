using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class DetailsModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IPictureService pictureService, ILogger<DetailsModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public PictureDto? Picture { get; set; }

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
                return NotFound();
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture details for ID {PictureId}", id);
            return NotFound();
        }
    }
}
