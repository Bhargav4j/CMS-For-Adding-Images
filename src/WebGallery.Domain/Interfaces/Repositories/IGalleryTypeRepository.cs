using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for GalleryType entity operations
/// </summary>
public interface IGalleryTypeRepository
{
    Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(GalleryType galleryType, CancellationToken cancellationToken = default);
    Task UpdateAsync(GalleryType galleryType, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
