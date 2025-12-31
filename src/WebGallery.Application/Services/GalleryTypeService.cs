using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

/// <summary>
/// Service implementation for GalleryType business logic
/// </summary>
public class GalleryTypeService : IGalleryTypeService
{
    private readonly IGalleryTypeRepository _repository;
    private readonly ILogger<GalleryTypeService> _logger;

    public GalleryTypeService(IGalleryTypeRepository repository, ILogger<GalleryTypeService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all gallery types");
            return await _repository.GetAllAsync(cancellationToken);
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
            _logger.LogInformation("Retrieving gallery type with ID: {Id}", id);
            return await _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<GalleryType> CreateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new gallery type: {Name}", galleryType.Name);
            galleryType.CreatedDate = DateTime.UtcNow;
            galleryType.IsActive = true;
            await _repository.AddAsync(galleryType, cancellationToken);
            _logger.LogInformation("Gallery type created successfully with ID: {Id}", galleryType.Id);
            return galleryType;
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

            var existingGalleryType = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingGalleryType == null)
            {
                throw new KeyNotFoundException($"Gallery type with ID {id} not found");
            }

            galleryType.Id = id;
            galleryType.ModifiedDate = DateTime.UtcNow;
            await _repository.UpdateAsync(galleryType, cancellationToken);
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
            await _repository.DeleteAsync(id, cancellationToken);
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
            return await _repository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
