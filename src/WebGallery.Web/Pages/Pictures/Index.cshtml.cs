using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebGallery.Application.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

public class IndexModel : PageModel
{
    private readonly IPictureService<PictureDto, PictureCreateDto, PictureUpdateDto> _pictureService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IPictureService<PictureDto, PictureCreateDto, PictureUpdateDto> pictureService, ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _logger = logger;
    }

    public IEnumerable<PictureDto> Pictures { get; set; } = new List<PictureDto>();

    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                Pictures = await _pictureService.SearchAsync(SearchString);
            }
            else
            {
                Pictures = await _pictureService.GetAllAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pictures");
            Pictures = new List<PictureDto>();
        }
    }
}
