using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

public class GalleryTypeService : IGalleryTypeService
{
    private readonly IGalleryTypeRepository _galleryTypeRepository;
    private readonly ILogger<GalleryTypeService> _logger;

    public GalleryTypeService(
        IGalleryTypeRepository galleryTypeRepository,
        ILogger<GalleryTypeService> logger)
    {
        _galleryTypeRepository = galleryTypeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all gallery types");
            return await _galleryTypeRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all gallery types");
            throw;
        }
    }

    public async Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving gallery type with ID: {GalleryTypeId}", id);
            return await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gallery type with ID: {GalleryTypeId}", id);
            throw;
        }
    }

    public async Task<GalleryType> CreateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new gallery type: {GalleryTypeName}", galleryType.Name);

            galleryType.CreatedDate = DateTime.UtcNow;
            galleryType.IsActive = true;

            var result = await _galleryTypeRepository.AddAsync(galleryType, cancellationToken);
            _logger.LogInformation("Gallery type created successfully with ID: {GalleryTypeId}", result.Id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type: {GalleryTypeName}", galleryType.Name);
            throw;
        }
    }

    public async Task UpdateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating gallery type with ID: {GalleryTypeId}", galleryType.Id);

            var existingGalleryType = await _galleryTypeRepository.GetByIdAsync(galleryType.Id, cancellationToken);
            if (existingGalleryType == null)
            {
                throw new InvalidOperationException($"Gallery type with ID {galleryType.Id} not found");
            }

            galleryType.ModifiedDate = DateTime.UtcNow;

            await _galleryTypeRepository.UpdateAsync(galleryType, cancellationToken);
            _logger.LogInformation("Gallery type updated successfully with ID: {GalleryTypeId}", galleryType.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type with ID: {GalleryTypeId}", galleryType.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting gallery type with ID: {GalleryTypeId}", id);

            var exists = await _galleryTypeRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException($"Gallery type with ID {id} not found");
            }

            await _galleryTypeRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Gallery type deleted successfully with ID: {GalleryTypeId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type with ID: {GalleryTypeId}", id);
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
