using GalleryApp.Domain.DTOs;

namespace GalleryApp.Domain.Interfaces.Services;

/// <summary>
/// Service interface for GalleryType operations
/// </summary>
public interface IGalleryTypeService
{
    Task<IEnumerable<GalleryTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GalleryTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<GalleryTypeDto> CreateAsync(GalleryTypeCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, GalleryTypeUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GalleryTypeDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
