using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gallery.Web.Pages.Pictures;

public class IndexModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<IndexModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    public IEnumerable<Picture> Pictures { get; set; } = new List<Picture>();
    public SelectList GalleryTypeSelectList { get; set; } = new SelectList(Enumerable.Empty<SelectListItem>());

    [BindProperty(SupportsGet = true)]
    public int? GalleryTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            // Load gallery types for dropdown
            var galleryTypes = await _galleryTypeService.GetAllAsync();
            var galleryTypeList = galleryTypes.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = g.Name
            }).ToList();
            galleryTypeList.Insert(0, new SelectListItem { Value = "", Text = "All Gallery Types" });
            GalleryTypeSelectList = new SelectList(galleryTypeList, "Value", "Text", GalleryTypeId?.ToString());

            // Load pictures
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Pictures = await _pictureService.SearchAsync(SearchTerm);
                if (GalleryTypeId.HasValue)
                {
                    Pictures = Pictures.Where(p => p.GalleryTypeId == GalleryTypeId.Value);
                }
            }
            else if (GalleryTypeId.HasValue)
            {
                Pictures = await _pictureService.GetByGalleryTypeAsync(GalleryTypeId.Value);
            }
            else
            {
                Pictures = await _pictureService.GetAllAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pictures");
        }
    }
}
