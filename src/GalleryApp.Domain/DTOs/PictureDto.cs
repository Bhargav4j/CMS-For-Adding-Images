namespace GalleryApp.Domain.DTOs;

/// <summary>
/// DTO for Picture entity
/// </summary>
public class PictureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
    public string? GalleryTypeName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new Picture
/// </summary>
public class PictureCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating an existing Picture
/// </summary>
public class PictureUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
}
