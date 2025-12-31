namespace Gallery.Domain.Entities;

/// <summary>
/// Represents a picture in the gallery
/// </summary>
public class Picture
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    // Navigation properties
    public virtual GalleryType? GalleryType { get; set; }
}
