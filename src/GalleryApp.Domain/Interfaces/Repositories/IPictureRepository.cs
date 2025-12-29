using GalleryApp.Domain.Entities;

namespace GalleryApp.Domain.Interfaces.Repositories;

public interface IPictureRepository
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default);
    Task<Picture> AddAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
