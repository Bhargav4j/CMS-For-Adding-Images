using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Interfaces.Repositories;

public interface IPictureRepository
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Picture> AddAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default);
}
