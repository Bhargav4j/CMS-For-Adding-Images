namespace WebGallery.Domain.Interfaces.Services;

/// <summary>
/// Service interface for GalleryType operations
/// </summary>
public interface IGalleryTypeService<TDto, TCreateDto, TUpdateDto>
{
    Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
