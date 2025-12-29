using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GalleryApp.Web.Pages;

public class GalleryModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<GalleryModel> _logger;

    public GalleryModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<GalleryModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public IEnumerable<Picture> Pictures { get; set; } = new List<Picture>();
    public SelectList GalleryTypes { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public int? GalleryTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync();
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name", GalleryTypeId);

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Pictures = await _pictureService.SearchAsync(SearchTerm);
            }
            else if (GalleryTypeId.HasValue)
            {
                Pictures = await _pictureService.GetByGalleryTypeIdAsync(GalleryTypeId.Value);
            }
            else
            {
                Pictures = await _pictureService.GetAllAsync();
            }

            _logger.LogInformation("Gallery page loaded with {Count} pictures", Pictures.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery page");
            Pictures = new List<Picture>();
        }
    }
}
