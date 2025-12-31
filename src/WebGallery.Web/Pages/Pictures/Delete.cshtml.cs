using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class DeleteModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IPictureService pictureService, ILogger<DeleteModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    [BindProperty]
    public PictureDto? Picture { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            Picture = await _pictureService.GetByIdAsync(id.Value);
            if (Picture == null)
            {
                return NotFound();
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture for delete with ID {PictureId}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Picture?.Id == null)
        {
            return NotFound();
        }

        try
        {
            await _pictureService.DeleteAsync(Picture.Id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture with ID {PictureId}", Picture.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the picture.");
            return Page();
        }
    }
}
