using System.ComponentModel.DataAnnotations;

namespace GalleryApp.Web.ViewModels;

/// <summary>
/// View model for GalleryType display and editing
/// </summary>
public class GalleryTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int PictureCount { get; set; }
}
