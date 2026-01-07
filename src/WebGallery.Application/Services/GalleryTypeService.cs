using AutoMapper;
using Microsoft.Extensions.Logging;
using WebGallery.Application.DTOs;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

/// <summary>
/// Service implementation for GalleryType operations
/// </summary>
public class GalleryTypeService : IGalleryTypeService<GalleryTypeDto, GalleryTypeCreateDto, GalleryTypeUpdateDto>
{
    private readonly IGalleryTypeRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GalleryTypeService> _logger;

    public GalleryTypeService(IGalleryTypeRepository repository, IMapper mapper, ILogger<GalleryTypeService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<GalleryTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all gallery types");
            var galleryTypes = await _repository.GetAllAsync(cancellationToken);
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
            _logger.LogInformation("Getting gallery type with ID: {Id}", id);
            var galleryType = await _repository.GetByIdAsync(id, cancellationToken);
            return galleryType != null ? _mapper.Map<GalleryTypeDto>(galleryType) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<GalleryTypeDto> CreateAsync(GalleryTypeCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new gallery type: {Name}", dto.Name);
            var galleryType = _mapper.Map<GalleryType>(dto);
            var createdGalleryType = await _repository.AddAsync(galleryType, cancellationToken);
            return _mapper.Map<GalleryTypeDto>(createdGalleryType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type: {Name}", dto.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, GalleryTypeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating gallery type with ID: {Id}", id);
            var existingGalleryType = await _repository.GetByIdAsync(id, cancellationToken);

            if (existingGalleryType == null)
            {
                throw new InvalidOperationException($"Gallery type with ID {id} not found");
            }

            _mapper.Map(dto, existingGalleryType);
            await _repository.UpdateAsync(existingGalleryType, cancellationToken);
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<GalleryTypeDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching gallery types with term: {SearchTerm}", searchTerm);
            var galleryTypes = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<GalleryTypeDto>>(galleryTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
