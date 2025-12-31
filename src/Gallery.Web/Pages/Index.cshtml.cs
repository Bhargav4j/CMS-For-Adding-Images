using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gallery.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IGalleryTypeService galleryTypeService, ILogger<IndexModel> logger)
    {
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public IEnumerable<GalleryType> GalleryTypes { get; set; } = new List<GalleryType>();

    public async Task OnGetAsync()
    {
        try
        {
            GalleryTypes = await _galleryTypeService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery types");
        }
    }
}
