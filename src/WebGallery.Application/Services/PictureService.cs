using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

/// <summary>
/// Service implementation for Picture business logic
/// </summary>
public class PictureService : IPictureService
{
    private readonly IPictureRepository _repository;
    private readonly ILogger<PictureService> _logger;

    public PictureService(IPictureRepository repository, ILogger<PictureService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all pictures");
            return await _repository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all pictures");
            throw;
        }
    }

    public async Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving picture with ID: {Id}", id);
            return await _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pictures for gallery type ID: {GalleryTypeId}", galleryTypeId);
            return await _repository.GetByGalleryTypeIdAsync(galleryTypeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pictures for gallery type ID: {GalleryTypeId}", galleryTypeId);
            throw;
        }
    }

    public async Task<Picture> CreateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new picture: {Name}", picture.Name);
            picture.CreatedDate = DateTime.UtcNow;
            picture.IsActive = true;
            await _repository.AddAsync(picture, cancellationToken);
            _logger.LogInformation("Picture created successfully with ID: {Id}", picture.Id);
            return picture;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture: {Name}", picture.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating picture with ID: {Id}", id);

            var existingPicture = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingPicture == null)
            {
                throw new KeyNotFoundException($"Picture with ID {id} not found");
            }

            picture.Id = id;
            picture.ModifiedDate = DateTime.UtcNow;
            await _repository.UpdateAsync(picture, cancellationToken);
            _logger.LogInformation("Picture updated successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting picture with ID: {Id}", id);
            await _repository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Picture deleted successfully with ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching pictures with term: {SearchTerm}", searchTerm);
            return await _repository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
