using GalleryApp.Domain.DTOs;

namespace GalleryApp.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Picture operations
/// </summary>
public interface IPictureService
{
    Task<IEnumerable<PictureDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PictureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PictureDto>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default);
    Task<PictureDto> CreateAsync(PictureCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, PictureUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PictureDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
