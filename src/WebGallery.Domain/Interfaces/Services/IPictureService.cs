using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Picture business logic operations
/// </summary>
public interface IPictureService
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default);
    Task<Picture> CreateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, Picture picture, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
