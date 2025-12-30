using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Interfaces.Services;
using GalleryApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace GalleryApp.Web.Pages.Admin.Pictures;

[Authorize(Roles = "admin")]
public class EditModel : PageModel
{
    private readonly IPictureService _pictureService;
    private readonly IGalleryTypeService _galleryTypeService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        IPictureService pictureService,
        IGalleryTypeService galleryTypeService,
        IWebHostEnvironment environment,
        ILogger<EditModel> logger)
    {
        _pictureService = pictureService;
        _galleryTypeService = galleryTypeService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public PictureViewModel Picture { get; set; } = new PictureViewModel();

    public SelectList GalleryTypes { get; set; } = new SelectList(new List<GalleryTypeDto>(), "Id", "Name");

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var pictureDto = await _pictureService.GetByIdAsync(id, cancellationToken);
            if (pictureDto == null)
            {
                return NotFound();
            }

            Picture = new PictureViewModel
            {
                Id = pictureDto.Id,
                Name = pictureDto.Name,
                Description = pictureDto.Description,
                ImagePath = pictureDto.ImagePath,
                ThumbnailImagePath = pictureDto.ThumbnailImagePath,
                GalleryTypeId = pictureDto.GalleryTypeId,
                GalleryTypeName = pictureDto.GalleryTypeName
            };

            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name", Picture.GalleryTypeId);

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit picture page for ID: {Id}", id);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name", Picture.GalleryTypeId);
            return Page();
        }

        try
        {
            var imagePath = Picture.ImagePath;
            var thumbnailPath = Picture.ThumbnailImagePath;

            if (Picture.ImageFile != null && Picture.ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(Picture.ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Picture.ImageFile", "Only image files are allowed.");
                    var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
                    GalleryTypes = new SelectList(galleryTypes, "Id", "Name", Picture.GalleryTypeId);
                    return Page();
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                var thumbnailsFolder = Path.Combine(_environment.WebRootPath, "images", "thumbnails");

                Directory.CreateDirectory(uploadsFolder);
                Directory.CreateDirectory(thumbnailsFolder);

                if (!string.IsNullOrEmpty(Picture.ImagePath))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, Picture.ImagePath.TrimStart('/'));
                    var oldThumbnailPath = Path.Combine(_environment.WebRootPath, Picture.ThumbnailImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);

                    if (System.IO.File.Exists(oldThumbnailPath))
                        System.IO.File.Delete(oldThumbnailPath);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                var thumbnailFilePath = Path.Combine(thumbnailsFolder, uniqueFileName);

                using (var image = await Image.LoadAsync(Picture.ImageFile.OpenReadStream()))
                {
                    await image.SaveAsync(filePath);

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(300, 225),
                        Mode = ResizeMode.Crop
                    }));
                    await image.SaveAsync(thumbnailFilePath);
                }

                imagePath = $"/images/{uniqueFileName}";
                thumbnailPath = $"/images/thumbnails/{uniqueFileName}";
            }

            var updateDto = new PictureUpdateDto
            {
                Name = Picture.Name,
                Description = Picture.Description,
                ImagePath = imagePath,
                ThumbnailImagePath = thumbnailPath,
                GalleryTypeId = Picture.GalleryTypeId,
                ModifiedBy = User.Identity?.Name ?? "admin"
            };

            await _pictureService.UpdateAsync(Picture.Id, updateDto, cancellationToken);

            _logger.LogInformation("Picture updated: {Id}", Picture.Id);

            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture: {Id}", Picture.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the picture.");
            var galleryTypes = await _galleryTypeService.GetAllAsync(cancellationToken);
            GalleryTypes = new SelectList(galleryTypes, "Id", "Name", Picture.GalleryTypeId);
            return Page();
        }
    }
}
