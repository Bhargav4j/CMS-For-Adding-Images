using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class PictureDetailsModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<PictureDetailsModel> _logger;

    public PictureDetailsModel(IPictureService pictureService, ILogger<PictureDetailsModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public Picture? Picture { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Picture = await _pictureService.GetByIdAsync(id, cancellationToken);

            if (Picture == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture details for ID: {PictureId}", id);
            return RedirectToPage("./Index");
        }
    }
}
