using System.ComponentModel.DataAnnotations;

namespace WebGallery.Web.ViewModels;

public class GalleryTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Gallery name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public int PictureCount { get; set; }

    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
