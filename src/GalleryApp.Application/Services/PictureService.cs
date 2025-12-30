using AutoMapper;
using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace GalleryApp.Application.Services;

/// <summary>
/// Service for Picture operations
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
        _pictureRepository = pictureRepository;
        _mapper = mapper;
        _logger = logger;
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
            _logger.LogInformation("Getting picture by ID: {Id}", id);
            var picture = await _pictureRepository.GetByIdAsync(id, cancellationToken);
            return picture != null ? _mapper.Map<PictureDto>(picture) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting picture by ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PictureDto>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pictures by gallery type ID: {GalleryTypeId}", galleryTypeId);
            var pictures = await _pictureRepository.GetByGalleryTypeIdAsync(galleryTypeId, cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pictures by gallery type ID: {GalleryTypeId}", galleryTypeId);
            throw;
        }
    }

    public async Task<PictureDto> CreateAsync(PictureCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new picture: {Name}", dto.Name);
            var picture = _mapper.Map<Picture>(dto);
            var createdPicture = await _pictureRepository.AddAsync(picture, cancellationToken);
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
            _logger.LogInformation("Updating picture: {Id}", id);
            var existingPicture = await _pictureRepository.GetByIdAsync(id, cancellationToken);
            if (existingPicture == null)
            {
                throw new InvalidOperationException($"Picture with ID {id} not found");
            }

            _mapper.Map(dto, existingPicture);
            await _pictureRepository.UpdateAsync(existingPicture, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting picture: {Id}", id);
            await _pictureRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PictureDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching pictures with term: {SearchTerm}", searchTerm);
            var pictures = await _pictureRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<PictureDto>>(pictures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
