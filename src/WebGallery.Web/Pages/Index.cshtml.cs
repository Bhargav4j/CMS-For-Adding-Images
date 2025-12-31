using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IPictureService pictureService, ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public IEnumerable<Picture> FeaturedPictures { get; set; } = new List<Picture>();

    public async Task OnGetAsync()
    {
        try
        {
            var allPictures = await _pictureService.GetAllAsync();
            FeaturedPictures = allPictures.Take(6);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured pictures");
            FeaturedPictures = new List<Picture>();
        }
    }
}
