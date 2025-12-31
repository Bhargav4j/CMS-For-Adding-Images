using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Picture entity operations
/// </summary>
public interface IPictureRepository
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default);
    Task AddAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
