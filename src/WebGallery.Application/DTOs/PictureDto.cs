namespace WebGallery.Application.DTOs;

/// <summary>
/// Data transfer object for Picture
/// </summary>
public class PictureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
    public string GalleryTypeName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

/// <summary>
/// DTO for creating a new Picture
/// </summary>
public class PictureCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
}

/// <summary>
/// DTO for updating a Picture
/// </summary>
public class PictureUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string ThumbnailImagePath { get; set; } = string.Empty;
    public int GalleryTypeId { get; set; }
}
