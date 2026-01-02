using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Interfaces.Services;
using WebGallery.Web.ViewModels;

namespace WebGallery.Web.Pages.Gallery;

public class GalleryDetailsModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<GalleryDetailsModel> _logger;

    public GalleryDetailsModel(
        IPictureService pictureService,
        ILogger<GalleryDetailsModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public PictureViewModel Picture { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var picture = await _pictureService.GetByIdAsync(id);

            if (picture == null)
            {
                _logger.LogWarning("Picture with ID {PictureId} not found", id);
                return NotFound();
            }

            Picture = new PictureViewModel
            {
                Id = picture.Id,
                Name = picture.Name,
                Description = picture.Description,
                ImagePath = picture.ImagePath,
                ThumbnailImagePath = picture.ThumbnailImagePath,
                GalleryId = picture.GalleryId,
                GalleryName = picture.GalleryType?.Name,
                CreatedDate = picture.CreatedDate,
                CreatedBy = picture.CreatedBy
            };

            _logger.LogInformation("Picture details loaded for ID {PictureId}", id);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture details for ID {PictureId}", id);
            return RedirectToPage("/Error");
        }
    }
}
