using System.ComponentModel.DataAnnotations;

namespace GalleryApp.Web.ViewModels;

/// <summary>
/// View model for Picture display and editing
/// </summary>
public class PictureViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Image is required")]
    public string ImagePath { get; set; } = string.Empty;

    public string ThumbnailImagePath { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gallery Type is required")]
    public int GalleryTypeId { get; set; }

    public string? GalleryTypeName { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public IFormFile? ImageFile { get; set; }
}
