using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Repositories;
using Gallery.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Gallery.Application.Services;

/// <summary>
/// Service implementation for Picture operations
/// </summary>
public class PictureService : IPictureService
{
    private readonly IPictureRepository _pictureRepository;
    private readonly ILogger<PictureService> _logger;

    public PictureService(
        IPictureRepository pictureRepository,
        ILogger<PictureService> logger)
    {
        _pictureRepository = pictureRepository ?? throw new ArgumentNullException(nameof(pictureRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all pictures");
            return await _pictureRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all pictures");
            throw;
        }
    }

    public async Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting picture with ID: {Id}", id);
            return await _pictureRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pictures for gallery type: {GalleryTypeId}", galleryTypeId);
            return await _pictureRepository.GetByGalleryTypeAsync(galleryTypeId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pictures for gallery type: {GalleryTypeId}", galleryTypeId);
            throw;
        }
    }

    public async Task<Picture> CreateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating picture: {Name}", picture.Name);

            picture.CreatedDate = DateTime.UtcNow;
            picture.IsActive = true;

            var result = await _pictureRepository.AddAsync(picture, cancellationToken);
            _logger.LogInformation("Picture created successfully with ID: {Id}", result.Id);
            return result;
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

            var existingPicture = await _pictureRepository.GetByIdAsync(id, cancellationToken);
            if (existingPicture == null)
            {
                throw new InvalidOperationException($"Picture with ID {id} not found");
            }

            picture.Id = id;
            picture.ModifiedDate = DateTime.UtcNow;
            picture.CreatedDate = existingPicture.CreatedDate;
            picture.CreatedBy = existingPicture.CreatedBy;

            await _pictureRepository.UpdateAsync(picture, cancellationToken);
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
            await _pictureRepository.DeleteAsync(id, cancellationToken);
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
            return await _pictureRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
