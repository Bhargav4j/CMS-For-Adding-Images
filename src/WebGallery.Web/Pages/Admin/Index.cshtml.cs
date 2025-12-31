using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Admin;

[Authorize(Roles = "Admin")]
public class AdminIndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<AdminIndexModel> _logger;

    public AdminIndexModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<AdminIndexModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public int TotalPictures { get; set; }
    public int TotalGalleryTypes { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var pictures = await _pictureService.GetAllAsync();
            var galleryTypes = await _galleryTypeService.GetAllAsync();

            TotalPictures = pictures.Count();
            TotalGalleryTypes = galleryTypes.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard data");
        }
    }
}
