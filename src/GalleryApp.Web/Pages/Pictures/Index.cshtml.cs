using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Web.Pages.Pictures;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IPictureService pictureService, ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public IEnumerable<Picture> Pictures { get; set; } = new List<Picture>();

    public async Task OnGetAsync()
    {
        try
        {
            Pictures = await _pictureService.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} pictures", Pictures.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pictures");
            Pictures = new List<Picture>();
        }
    }
}
