using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Repositories;
using Gallery.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Gallery.Application.Services;

/// <summary>
/// Service implementation for GalleryType operations
/// </summary>
public class GalleryTypeService : IGalleryTypeService
{
    private readonly IGalleryTypeRepository _galleryTypeRepository;
    private readonly ILogger<GalleryTypeService> _logger;

    public GalleryTypeService(
        IGalleryTypeRepository galleryTypeRepository,
        ILogger<GalleryTypeService> logger)
    {
        _galleryTypeRepository = galleryTypeRepository ?? throw new ArgumentNullException(nameof(galleryTypeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all gallery types");
            return await _galleryTypeRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all gallery types");
            throw;
        }
    }

    public async Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting gallery type with ID: {Id}", id);
            return await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<GalleryType> CreateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating gallery type: {Name}", galleryType.Name);

            galleryType.CreatedDate = DateTime.UtcNow;
            galleryType.IsActive = true;

            var result = await _galleryTypeRepository.AddAsync(galleryType, cancellationToken);
            _logger.LogInformation("Gallery type created successfully with ID: {Id}", result.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type: {Name}", galleryType.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating gallery type with ID: {Id}", id);

            var existingGalleryType = await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
            if (existingGalleryType == null)
            {
                throw new InvalidOperationException($"Gallery type with ID {id} not found");
            }

            galleryType.Id = id;
            galleryType.ModifiedDate = DateTime.UtcNow;
            galleryType.CreatedDate = existingGalleryType.CreatedDate;
            galleryType.CreatedBy = existingGalleryType.CreatedBy;

            await _galleryTypeRepository.UpdateAsync(galleryType, cancellationToken);
            _logger.LogInformation("Gallery type updated successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting gallery type with ID: {Id}", id);
            await _galleryTypeRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Gallery type deleted successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching gallery types with term: {SearchTerm}", searchTerm);
            return await _galleryTypeRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
