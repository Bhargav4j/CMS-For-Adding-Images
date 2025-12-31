using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gallery.Web.Pages.Pictures;

[Authorize(Roles = "admin")]
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
    public Picture? Picture { get; set; }

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
            _logger.LogError(ex, "Error loading picture for delete: {Id}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Picture == null || Picture.Id == 0)
        {
            return NotFound();
        }

        try
        {
            await _pictureService.DeleteAsync(Picture.Id);
            _logger.LogInformation("Picture deleted: {Id}", Picture.Id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture: {Id}", Picture.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the picture.");
            return Page();
        }
    }
}
