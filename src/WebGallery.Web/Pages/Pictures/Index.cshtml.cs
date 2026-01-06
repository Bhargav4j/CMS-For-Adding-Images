using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class PicturesIndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<PicturesIndexModel> _logger;

    public PicturesIndexModel(IPictureService pictureService, ILogger<PicturesIndexModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public IEnumerable<Picture> Pictures { get; set; } = new List<Picture>();

    [BindProperty(SupportsGet = true)]
    public int? GalleryTypeId { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (GalleryTypeId.HasValue)
            {
                Pictures = await _pictureService.GetByGalleryTypeAsync(GalleryTypeId.Value, cancellationToken);
            }
            else
            {
                Pictures = await _pictureService.GetAllAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pictures");
            Pictures = new List<Picture>();
        }
    }
}
