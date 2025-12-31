using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.GalleryTypes;

public class EditModel : PageModel
{
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IGalleryTypeService galleryTypeService, ILogger<EditModel> logger)
    {
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var galleryType = await _galleryTypeService.GetByIdAsync(id.Value);
            if (galleryType == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = galleryType.Id,
                Name = galleryType.Name,
                Description = galleryType.Description
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading gallery type for edit with ID {GalleryTypeId}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var dto = new GalleryTypeUpdateDto
            {
                Name = Input.Name,
                Description = Input.Description
            };

            await _galleryTypeService.UpdateAsync(Input.Id, dto);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type with ID {GalleryTypeId}", Input.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the gallery type.");
            return Page();
        }
    }
}
