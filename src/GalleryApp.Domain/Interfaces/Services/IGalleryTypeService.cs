using GalleryApp.Domain.Entities;

namespace GalleryApp.Domain.Interfaces.Services;

public interface IGalleryTypeService
{
    Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<GalleryType> CreateAsync(GalleryType galleryType, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, GalleryType galleryType, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
