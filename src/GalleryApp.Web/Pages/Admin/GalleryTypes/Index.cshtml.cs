using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Admin.GalleryTypes;

[Authorize(Roles = "admin")]
public class IndexModel : PageModel
{
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IGalleryTypeService galleryTypeService, ILogger<IndexModel> logger)
    {
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public IEnumerable<GalleryTypeDto> GalleryTypes { get; set; } = new List<GalleryTypeDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            GalleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery types");
            return RedirectToPage("/Error");
        }
    }
}
