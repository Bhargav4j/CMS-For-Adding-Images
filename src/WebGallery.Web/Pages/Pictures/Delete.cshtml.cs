using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

[Authorize(Roles = "admin")]
public class PictureDeleteModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<PictureDeleteModel> _logger;

    public PictureDeleteModel(IPictureService pictureService, ILogger<PictureDeleteModel> logger)
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
            _logger.LogError(ex, "Error loading picture for deletion");
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _pictureService.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Picture deleted: {PictureId}", id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture");
            return RedirectToPage("./Index");
        }
    }
}
