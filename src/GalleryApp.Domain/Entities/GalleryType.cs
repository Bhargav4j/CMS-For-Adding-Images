namespace GalleryApp.Domain.Entities;

/// <summary>
/// Represents a gallery category/type
/// </summary>
public class GalleryType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Picture> Pictures { get; set; } = new List<Picture>();
}
