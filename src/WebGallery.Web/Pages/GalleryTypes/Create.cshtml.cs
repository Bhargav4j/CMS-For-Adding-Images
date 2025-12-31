using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.GalleryTypes;

public class CreateModel : PageModel
{
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IGalleryTypeService galleryTypeService, ILogger<CreateModel> logger)
    {
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var dto = new GalleryTypeCreateDto
            {
                Name = Input.Name,
                Description = Input.Description
            };

            await _galleryTypeService.CreateAsync(dto);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the gallery type.");
            return Page();
        }
    }
}
