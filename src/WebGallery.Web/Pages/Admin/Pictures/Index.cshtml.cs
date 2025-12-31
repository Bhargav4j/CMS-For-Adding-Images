using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Admin.Pictures;

[Authorize(Roles = "Admin")]
public class AdminPicturesIndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<AdminPicturesIndexModel> _logger;

    public AdminPicturesIndexModel(IPictureService pictureService, ILogger<AdminPicturesIndexModel> logger)
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pictures for admin");
        }
    }
}
