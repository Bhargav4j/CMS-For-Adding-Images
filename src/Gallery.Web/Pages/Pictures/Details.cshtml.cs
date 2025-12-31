using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gallery.Web.Pages.Pictures;

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
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture details for ID: {Id}", id);
            return RedirectToPage("./Index");
        }
    }
}
