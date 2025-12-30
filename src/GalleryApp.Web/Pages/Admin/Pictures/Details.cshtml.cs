using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Admin.Pictures;

[Authorize(Roles = "admin")]
public class DetailsModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IPictureService pictureService, ILogger<DetailsModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public PictureDto Picture { get; set; } = new PictureDto();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var picture = await _pictureService.GetByIdAsync(id, cancellationToken);
            if (picture == null)
            {
                return NotFound();
            }

            Picture = picture;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture details for ID: {Id}", id);
            return RedirectToPage("/Error");
        }
    }
}
