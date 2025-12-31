using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class IndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IPictureService pictureService, ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public IEnumerable<PictureDto> Pictures { get; set; } = new List<PictureDto>();

    public async Task OnGetAsync()
    {
        try
        {
            Pictures = await _pictureService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pictures");
            Pictures = new List<PictureDto>();
        }
    }
}
