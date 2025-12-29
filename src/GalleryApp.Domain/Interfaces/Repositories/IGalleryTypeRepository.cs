using GalleryApp.Domain.Entities;

namespace GalleryApp.Domain.Interfaces.Repositories;

public interface IGalleryTypeRepository
{
    Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<GalleryType> AddAsync(GalleryType galleryType, CancellationToken cancellationToken = default);
    Task UpdateAsync(GalleryType galleryType, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
