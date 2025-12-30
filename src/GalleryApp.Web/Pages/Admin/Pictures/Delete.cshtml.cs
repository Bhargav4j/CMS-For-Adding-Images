using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Admin.Pictures;

[Authorize(Roles = "admin")]
public class DeleteModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(
        IPictureService pictureService,
        IWebHostEnvironment environment,
        ILogger<DeleteModel> logger)
    {
        _pictureService = pictureService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public PictureDto Picture { get; set; } = new PictureDto();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var picture = await _pictureService.GetByIdAsync(id, cancellationToken);
            if (picture == null)
            {
                return NotFound();
            }

            Picture = picture;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete picture page for ID: {Id}", id);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var picture = await _pictureService.GetByIdAsync(Picture.Id, cancellationToken);
            if (picture != null)
            {
                if (!string.IsNullOrEmpty(picture.ImagePath))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, picture.ImagePath.TrimStart('/'));
                    var thumbnailPath = Path.Combine(_environment.WebRootPath, picture.ThumbnailImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);

                    if (System.IO.File.Exists(thumbnailPath))
                        System.IO.File.Delete(thumbnailPath);
                }

                await _pictureService.DeleteAsync(Picture.Id, cancellationToken);
                _logger.LogInformation("Picture deleted: {Id}", Picture.Id);
            }

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture: {Id}", Picture.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the picture.");
            return Page();
        }
    }
}
