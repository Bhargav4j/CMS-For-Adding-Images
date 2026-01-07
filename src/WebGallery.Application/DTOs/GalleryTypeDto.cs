namespace WebGallery.Application.DTOs;

/// <summary>
/// Data transfer object for GalleryType
/// </summary>
public class GalleryTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int PictureCount { get; set; }
}

/// <summary>
/// DTO for creating a new GalleryType
/// </summary>
public class GalleryTypeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a GalleryType
/// </summary>
public class GalleryTypeUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
