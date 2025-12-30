using AutoMapper;
using GalleryApp.Domain.DTOs;
using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace GalleryApp.Application.Services;

/// <summary>
/// Service for GalleryType operations
/// </summary>
public class GalleryTypeService : IGalleryTypeService
{
    private readonly IGalleryTypeRepository _galleryTypeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GalleryTypeService> _logger;

    public GalleryTypeService(
        IGalleryTypeRepository galleryTypeRepository,
        IMapper mapper,
        ILogger<GalleryTypeService> logger)
    {
        _galleryTypeRepository = galleryTypeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<GalleryTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all gallery types");
            var galleryTypes = await _galleryTypeRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<GalleryTypeDto>>(galleryTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all gallery types");
            throw;
        }
    }

    public async Task<GalleryTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting gallery type by ID: {Id}", id);
            var galleryType = await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
            return galleryType != null ? _mapper.Map<GalleryTypeDto>(galleryType) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gallery type by ID: {Id}", id);
            throw;
        }
    }

    public async Task<GalleryTypeDto> CreateAsync(GalleryTypeCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new gallery type: {Name}", dto.Name);
            var galleryType = _mapper.Map<GalleryType>(dto);
            var createdGalleryType = await _galleryTypeRepository.AddAsync(galleryType, cancellationToken);
            return _mapper.Map<GalleryTypeDto>(createdGalleryType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type: {Name}", dto.Name);
            throw;
        }
    }

    public async Task<IEnumerable<GalleryTypeDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching gallery types with term: {SearchTerm}", searchTerm);
            var galleryTypes = await _galleryTypeRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<GalleryTypeDto>>(galleryTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task UpdateAsync(int id, GalleryTypeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating gallery type: {Id}", id);
            var existingGalleryType = await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
            if (existingGalleryType == null)
            {
                throw new InvalidOperationException($"Gallery type with ID {id} not found");
            }

            _mapper.Map(dto, existingGalleryType);
            await _galleryTypeRepository.UpdateAsync(existingGalleryType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting gallery type: {Id}", id);
            await _galleryTypeRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type: {Id}", id);
            throw;
        }
    }
}
