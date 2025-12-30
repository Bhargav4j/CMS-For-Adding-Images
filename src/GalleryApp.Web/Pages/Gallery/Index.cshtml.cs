using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Gallery;

public class IndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public IEnumerable<PictureDto> Pictures { get; set; } = new List<PictureDto>();
    public IEnumerable<GalleryTypeDto> GalleryTypes { get; set; } = new List<GalleryTypeDto>();
    public int SelectedGalleryTypeId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? galleryId, CancellationToken cancellationToken = default)
    {
        try
        {
            GalleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);

            SelectedGalleryTypeId = galleryId ?? GalleryTypes.FirstOrDefault()?.Id ?? 1;

            Pictures = await _pictureService.GetByGalleryTypeIdAsync(SelectedGalleryTypeId, cancellationToken);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery page");
            return RedirectToPage("/Error");
        }
    }
}
