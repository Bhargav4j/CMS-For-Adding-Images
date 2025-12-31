using AutoMapper;
using Microsoft.Extensions.Logging;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

/// <summary>
/// Service implementation for Picture business logic
/// </summary>
public class PictureService : IPictureService
{
    private readonly IPictureRepository _pictureRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PictureService> _logger;

    public PictureService(
        IPictureRepository pictureRepository,
        IMapper mapper,
        ILogger<PictureService> logger)
    {
        _pictureRepository = pictureRepository ?? throw new ArgumentNullException(nameof(pictureRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<PictureDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all pictures");
            var pictures = await _pictureRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all pictures");
            throw;
        }
    }

    public async Task<PictureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting picture with ID {PictureId}", id);
            var picture = await _pictureRepository.GetByIdAsync(id, cancellationToken);
            return picture == null ? null : _mapper.Map<PictureDto>(picture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting picture with ID {PictureId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PictureDto>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pictures for gallery type {GalleryTypeId}", galleryTypeId);
            var pictures = await _pictureRepository.GetByGalleryTypeAsync(galleryTypeId, cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pictures for gallery type {GalleryTypeId}", galleryTypeId);
            throw;
        }
    }

    public async Task<PictureDto> CreateAsync(PictureCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating picture {PictureName}", dto.Name);
            var picture = _mapper.Map<Picture>(dto);
            picture.CreatedDate = DateTime.UtcNow;
            picture.IsActive = true;
            picture.CreatedBy = "system";

            var created = await _pictureRepository.AddAsync(picture, cancellationToken);
            return _mapper.Map<PictureDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture {PictureName}", dto.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, PictureUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating picture {PictureId}", id);
            var picture = await _pictureRepository.GetByIdAsync(id, cancellationToken);
            if (picture == null)
            {
                throw new InvalidOperationException($"Picture with ID {id} not found");
            }

            _mapper.Map(dto, picture);
            picture.ModifiedDate = DateTime.UtcNow;
            picture.ModifiedBy = "system";

            await _pictureRepository.UpdateAsync(picture, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture {PictureId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting picture {PictureId}", id);
            await _pictureRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture {PictureId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PictureDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching pictures with term {SearchTerm}", searchTerm);
            var pictures = await _pictureRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
