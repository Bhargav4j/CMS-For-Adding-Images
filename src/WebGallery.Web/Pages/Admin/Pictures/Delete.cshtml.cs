using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Admin.Pictures;

[Authorize(Roles = "Admin")]
public class AdminPicturesDeleteModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<AdminPicturesDeleteModel> _logger;

    public AdminPicturesDeleteModel(
        IPictureService pictureService,
        IWebHostEnvironment environment,
        ILogger<AdminPicturesDeleteModel> logger)
    {
        _pictureService = pictureService;
        _environment = environment;
        _logger = logger;
    }

    public Picture? Picture { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Picture = await _pictureService.GetByIdAsync(id);

        if (Picture == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            Picture = await _pictureService.GetByIdAsync(id);

            if (Picture == null)
            {
                return NotFound();
            }

            // Delete physical files
            try
            {
                var largeImagePath = Path.Combine(_environment.WebRootPath, Picture.ImagePath.TrimStart('/'));
                var thumbPath = Path.Combine(_environment.WebRootPath, Picture.ThumbnailImagePath.TrimStart('/'));

                if (System.IO.File.Exists(largeImagePath))
                {
                    System.IO.File.Delete(largeImagePath);
                }

                if (System.IO.File.Exists(thumbPath))
                {
                    System.IO.File.Delete(thumbPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not delete physical files for picture {Id}", id);
            }

            // Delete from database (soft delete)
            await _pictureService.DeleteAsync(id);

            _logger.LogInformation("Picture deleted: {Id}", id);

            return RedirectToPage("/Admin/Pictures/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture {Id}", id);
            return RedirectToPage("/Admin/Pictures/Index");
        }
    }
}
