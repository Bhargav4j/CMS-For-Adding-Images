using WebGallery.Domain.Entities;

namespace WebGallery.Domain.Interfaces.Services;

public interface IPictureService
{
    Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Picture> CreateAsync(Picture picture, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, Picture picture, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Picture>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default);
}
