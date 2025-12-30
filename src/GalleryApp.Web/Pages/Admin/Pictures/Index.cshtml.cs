using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Admin.Pictures;

[Authorize(Roles = "admin")]
public class IndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IPictureService pictureService, ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public IEnumerable<PictureDto> Pictures { get; set; } = new List<PictureDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Pictures = await _pictureService.GetAllAsync(cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pictures");
            return RedirectToPage("/Error");
        }
    }
}
