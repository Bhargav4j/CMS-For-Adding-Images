namespace GalleryApp.Domain.DTOs;

/// <summary>
/// DTO for GalleryType entity
/// </summary>
public class GalleryTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }
    public bool IsActive { get; set; }
    public int PictureCount { get; set; }
}

/// <summary>
/// DTO for creating a new GalleryType
/// </summary>
public class GalleryTypeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an existing GalleryType
/// </summary>
public class GalleryTypeUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
