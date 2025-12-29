using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace GalleryApp.Application.Services;

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

    public async Task<IEnumerable<Picture>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pictures for gallery type ID: {GalleryTypeId}", galleryTypeId);
            return await _pictureRepository.GetByGalleryTypeIdAsync(galleryTypeId, cancellationToken);
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
            _logger.LogInformation("Creating new picture: {PictureName}", picture.Name);
            picture.CreatedDate = DateTime.UtcNow;
            picture.IsActive = true;
            return await _pictureRepository.AddAsync(picture, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture: {PictureName}", picture.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating picture with ID: {PictureId}", id);

            var existingPicture = await _pictureRepository.GetByIdAsync(id, cancellationToken);
            if (existingPicture == null)
            {
                throw new InvalidOperationException($"Picture with ID {id} not found");
            }

            existingPicture.Name = picture.Name;
            existingPicture.Description = picture.Description;
            existingPicture.ImagePath = picture.ImagePath;
            existingPicture.ThumbnailImagePath = picture.ThumbnailImagePath;
            existingPicture.GalleryTypeId = picture.GalleryTypeId;
            existingPicture.ModifiedDate = DateTime.UtcNow;
            existingPicture.ModifiedBy = picture.ModifiedBy;

            await _pictureRepository.UpdateAsync(existingPicture, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture with ID: {PictureId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting picture with ID: {PictureId}", id);
            await _pictureRepository.DeleteAsync(id, cancellationToken);
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
