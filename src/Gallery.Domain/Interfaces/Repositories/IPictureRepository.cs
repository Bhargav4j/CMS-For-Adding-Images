using Gallery.Domain.Entities;

namespace Gallery.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Picture entity
/// </summary>
public interface IPictureRepository
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default);
    Task<Picture> AddAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
