using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace GalleryApp.Application.Services;

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
            return await _galleryTypeRepository.AddAsync(galleryType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type: {GalleryTypeName}", galleryType.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating gallery type with ID: {GalleryTypeId}", id);

            var existingGalleryType = await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
            if (existingGalleryType == null)
            {
                throw new InvalidOperationException($"Gallery type with ID {id} not found");
            }

            existingGalleryType.Name = galleryType.Name;
            existingGalleryType.Description = galleryType.Description;
            existingGalleryType.ModifiedDate = DateTime.UtcNow;
            existingGalleryType.ModifiedBy = galleryType.ModifiedBy;

            await _galleryTypeRepository.UpdateAsync(existingGalleryType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type with ID: {GalleryTypeId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting gallery type with ID: {GalleryTypeId}", id);
            await _galleryTypeRepository.DeleteAsync(id, cancellationToken);
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
