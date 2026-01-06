namespace WebGallery.Domain.Entities;

public class Picture
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? ThumbnailImagePath { get; set; }
    public int GalleryTypeId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }

    public virtual GalleryType? GalleryType { get; set; }
}
