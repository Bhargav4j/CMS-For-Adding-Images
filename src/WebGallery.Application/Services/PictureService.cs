using AutoMapper;
using Microsoft.Extensions.Logging;
using WebGallery.Application.DTOs;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

/// <summary>
/// Service implementation for Picture operations
/// </summary>
public class PictureService : IPictureService
{
    private readonly IPictureRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<PictureService> _logger;

    public PictureService(IPictureRepository repository, IMapper mapper, ILogger<PictureService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<PictureDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all pictures");
            var pictures = await _repository.GetAllAsync(cancellationToken);
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
            _logger.LogInformation("Getting picture with ID: {Id}", id);
            var picture = await _repository.GetByIdAsync(id, cancellationToken);
            return picture != null ? _mapper.Map<PictureDto>(picture) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task<PictureDto> CreateAsync(PictureCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new picture: {Name}", dto.Name);
            var picture = _mapper.Map<Picture>(dto);
            var createdPicture = await _repository.AddAsync(picture, cancellationToken);
            return _mapper.Map<PictureDto>(createdPicture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating picture: {Name}", dto.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, PictureUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating picture with ID: {Id}", id);
            var existingPicture = await _repository.GetByIdAsync(id, cancellationToken);

            if (existingPicture == null)
            {
                throw new InvalidOperationException($"Picture with ID {id} not found");
            }

            _mapper.Map(dto, existingPicture);
            await _repository.UpdateAsync(existingPicture, cancellationToken);
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PictureDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching pictures with term: {SearchTerm}", searchTerm);
            var pictures = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<IEnumerable<PictureDto>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pictures for gallery type: {GalleryTypeId}", galleryTypeId);
            var pictures = await _repository.GetByGalleryTypeAsync(galleryTypeId, cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pictures for gallery type: {GalleryTypeId}", galleryTypeId);
            throw;
        }
    }
}
