using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.GalleryTypes;

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

    public async Task OnGetAsync()
    {
        try
        {
            GalleryTypes = await _galleryTypeService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery types");
            GalleryTypes = new List<GalleryTypeDto>();
        }
    }
}
