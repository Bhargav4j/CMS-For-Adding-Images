using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

public class PictureService : IPictureService
{
    private readonly IPictureRepository _pictureRepository;
    private readonly ILogger<PictureService> _logger;

    public PictureService(
        IPictureRepository pictureRepository,
        ILogger<PictureService> logger)
    {
        _pictureRepository = pictureRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all pictures");
            return await _pictureRepository.GetAllAsync(cancellationToken);
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
            _logger.LogInformation("Retrieving picture with ID: {PictureId}", id);
            return await _pictureRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving picture with ID: {PictureId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> GetByGalleryIdAsync(int galleryId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pictures for gallery ID: {GalleryId}", galleryId);
            return await _pictureRepository.GetByGalleryIdAsync(galleryId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pictures for gallery ID: {GalleryId}", galleryId);
            throw;
        }
    }

    public async Task<Picture> CreateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new picture: {PictureName}", picture.Name);

            picture.CreatedDate = DateTime.UtcNow;
            picture.IsActive = true;

            var result = await _pictureRepository.AddAsync(picture, cancellationToken);
            _logger.LogInformation("Picture created successfully with ID: {PictureId}", result.Id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture: {PictureName}", picture.Name);
            throw;
        }
    }

    public async Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating picture with ID: {PictureId}", picture.Id);

            var existingPicture = await _pictureRepository.GetByIdAsync(picture.Id, cancellationToken);
            if (existingPicture == null)
            {
                throw new InvalidOperationException($"Picture with ID {picture.Id} not found");
            }

            picture.ModifiedDate = DateTime.UtcNow;

            await _pictureRepository.UpdateAsync(picture, cancellationToken);
            _logger.LogInformation("Picture updated successfully with ID: {PictureId}", picture.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture with ID: {PictureId}", picture.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting picture with ID: {PictureId}", id);

            var exists = await _pictureRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException($"Picture with ID {id} not found");
            }

            await _pictureRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Picture deleted successfully with ID: {PictureId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture with ID: {PictureId}", id);
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
