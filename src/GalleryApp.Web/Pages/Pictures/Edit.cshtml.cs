using System.ComponentModel.DataAnnotations;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Services;
using GalleryApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GalleryApp.Web.Pages.Pictures;

[Authorize(Policy = "AdminOnly")]
public class EditModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IPathService _pathService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IPathService pathService,
        ILogger<EditModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _pathService = pathService;
        _logger = logger;
    }

    [BindProperty]
    public Picture Picture { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public SelectList GalleryTypes { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public int GalleryTypeId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Picture = await _pictureService.GetByIdAsync(id.Value);

        if (Picture == null)
        {
            return NotFound();
        }

        Input.Name = Picture.Name;
        Input.Description = Picture.Description;
        Input.GalleryTypeId = Picture.GalleryTypeId;

        await LoadGalleryTypesAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Picture = await _pictureService.GetByIdAsync(Picture.Id);
            await LoadGalleryTypesAsync();
            return Page();
        }

        try
        {
            var existingPicture = await _pictureService.GetByIdAsync(Picture.Id);
            if (existingPicture == null)
            {
                return NotFound();
            }

            existingPicture.Name = Input.Name;
            existingPicture.Description = Input.Description;
            existingPicture.GalleryTypeId = Input.GalleryTypeId;
            existingPicture.ModifiedBy = User.Identity?.Name ?? "System";

            if (Input.ImageFile != null && Input.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_pathService.GetWebRootPath(), "images", "uploads");
                var thumbnailsFolder = Path.Combine(_pathService.GetWebRootPath(), "images", "thumbnails");

                Directory.CreateDirectory(uploadsFolder);
                Directory.CreateDirectory(thumbnailsFolder);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                var thumbnailPath = Path.Combine(thumbnailsFolder, $"thumb_{fileName}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(stream);
                }

                using (var image = await Image.LoadAsync(Input.ImageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(200, 200),
                        Mode = ResizeMode.Crop
                    }));
                    await image.SaveAsync(thumbnailPath);
                }

                var oldImagePath = Path.Combine(_pathService.GetWebRootPath(), existingPicture.ImagePath.TrimStart('/'));
                var oldThumbnailPath = Path.Combine(_pathService.GetWebRootPath(), existingPicture.ThumbnailImagePath.TrimStart('/'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                if (System.IO.File.Exists(oldThumbnailPath))
                {
                    System.IO.File.Delete(oldThumbnailPath);
                }

                existingPicture.ImagePath = $"/images/uploads/{fileName}";
                existingPicture.ThumbnailImagePath = $"/images/thumbnails/thumb_{fileName}";
            }

            await _pictureService.UpdateAsync(Picture.Id, existingPicture);

            _logger.LogInformation("Picture updated successfully: {PictureId}", Picture.Id);

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture: {PictureId}", Picture.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the picture");
            Picture = await _pictureService.GetByIdAsync(Picture.Id);
            await LoadGalleryTypesAsync();
            return Page();
        }
    }

    private async Task LoadGalleryTypesAsync()
    {
        var galleryTypes = await _galleryTypeService.GetAllAsync();
        GalleryTypes = new SelectList(galleryTypes, "Id", "Name");
    }
}
