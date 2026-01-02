using System.ComponentModel.DataAnnotations;

namespace WebGallery.Web.ViewModels;

public class PictureViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Picture name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Image path is required")]
    public string ImagePath { get; set; } = string.Empty;

    public string ThumbnailImagePath { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gallery is required")]
    public int GalleryId { get; set; }

    public string? GalleryName { get; set; }

    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
