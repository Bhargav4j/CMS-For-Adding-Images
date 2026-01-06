using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Web.Pages.Pictures;

[Authorize(Roles = "admin")]
public class PictureEditModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly ILogger<PictureEditModel> _logger;

    public PictureEditModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        ILogger<PictureEditModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypes { get; set; } = new SelectList(new List<GalleryType>(), "Id", "Name");

    public class InputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Gallery Type")]
        public int GalleryTypeId { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var picture = await _pictureService.GetByIdAsync(id, cancellationToken);
            if (picture == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = picture.Id,
                Name = picture.Name,
                Description = picture.Description,
                GalleryTypeId = picture.GalleryTypeId
            };

            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading picture for edit");
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
            return Page();
        }

        try
        {
            var existingPicture = await _pictureService.GetByIdAsync(Input.Id, cancellationToken);
            if (existingPicture == null)
            {
                return NotFound();
            }

            var updatedPicture = new Picture
            {
                Id = Input.Id,
                Name = Input.Name,
                Description = Input.Description,
                GalleryTypeId = Input.GalleryTypeId,
                ImagePath = existingPicture.ImagePath,
                ThumbnailImagePath = existingPicture.ThumbnailImagePath,
                ModifiedBy = User.Identity?.Name ?? "Unknown"
            };

            await _pictureService.UpdateAsync(Input.Id, updatedPicture, cancellationToken);

            _logger.LogInformation("Picture updated: {PictureId}", Input.Id);

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the picture.");

            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
            return Page();
        }
    }
}
