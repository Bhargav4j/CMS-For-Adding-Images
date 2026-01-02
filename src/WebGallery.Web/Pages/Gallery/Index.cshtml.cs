using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Interfaces.Services;
using WebGallery.Web.ViewModels;

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

    public List<PictureViewModel> Pictures { get; set; } = new();
    public List<GalleryTypeViewModel> GalleryTypes { get; set; } = new();
    public int SelectedGalleryId { get; set; }

    public async Task<IActionResult> OnGetAsync(int? galleryId)
    {
        try
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync();
            GalleryTypes = galleryTypes.Select(g => new GalleryTypeViewModel
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreatedDate = g.CreatedDate,
                CreatedBy = g.CreatedBy
            }).ToList();

            SelectedGalleryId = galleryId ?? GalleryTypes.FirstOrDefault()?.Id ?? 1;

            var pictures = await _pictureService.GetByGalleryIdAsync(SelectedGalleryId);
            Pictures = pictures.Select(p => new PictureViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ImagePath = p.ImagePath,
                ThumbnailImagePath = p.ThumbnailImagePath,
                GalleryId = p.GalleryId,
                GalleryName = p.GalleryType?.Name,
                CreatedDate = p.CreatedDate,
                CreatedBy = p.CreatedBy
            }).ToList();

            _logger.LogInformation("Gallery page loaded with {Count} pictures for gallery ID {GalleryId}", Pictures.Count, SelectedGalleryId);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery page");
            return RedirectToPage("/Error");
        }
    }
}
