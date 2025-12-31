using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Gallery;

public class GalleryIndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<GalleryIndexModel> _logger;

    public GalleryIndexModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<GalleryIndexModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public IEnumerable<Picture> Pictures { get; set; } = new List<Picture>();
    public IEnumerable<GalleryType> GalleryTypes { get; set; } = new List<GalleryType>();
    public int? SelectedGalleryTypeId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? galleryTypeId)
    {
        try
        {
            SelectedGalleryTypeId = galleryTypeId;
            GalleryTypes = await _galleryTypeService.GetAllAsync();

            if (galleryTypeId.HasValue)
            {
                Pictures = await _pictureService.GetByGalleryTypeIdAsync(galleryTypeId.Value);
            }
            else
            {
                Pictures = await _pictureService.GetAllAsync();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery");
            return Page();
        }
    }
}
