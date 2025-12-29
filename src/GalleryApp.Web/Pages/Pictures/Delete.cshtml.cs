using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;
using GalleryApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Pictures;

[Authorize(Policy = "AdminOnly")]
public class DeleteModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IPathService _pathService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(
        IPictureService pictureService,
        IPathService pathService,
        ILogger<DeleteModel> logger)
    {
        _pictureService = pictureService;
        _pathService = pathService;
        _logger = logger;
    }

    [BindProperty]
    public Picture Picture { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Picture = await _pictureService.GetByIdAsync(id.Value) ?? new Picture();

        if (Picture.Id == 0)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var picture = await _pictureService.GetByIdAsync(Picture.Id);
            if (picture == null)
            {
                return NotFound();
            }

            var imagePath = Path.Combine(_pathService.GetWebRootPath(), picture.ImagePath.TrimStart('/'));
            var thumbnailPath = Path.Combine(_pathService.GetWebRootPath(), picture.ThumbnailImagePath.TrimStart('/'));

            await _pictureService.DeleteAsync(Picture.Id);

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            if (System.IO.File.Exists(thumbnailPath))
            {
                System.IO.File.Delete(thumbnailPath);
            }

            _logger.LogInformation("Picture deleted successfully: {PictureId}", Picture.Id);

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture: {PictureId}", Picture.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the picture");
            return Page();
        }
    }
}
