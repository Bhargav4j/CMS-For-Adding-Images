using AutoMapper;
using Microsoft.Extensions.Logging;
using WebGallery.Domain.DTOs;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Domain.Interfaces.Services;

namespace WebGallery.Application.Services;

/// <summary>
/// Service implementation for GalleryType business logic
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
        _galleryTypeRepository = galleryTypeRepository ?? throw new ArgumentNullException(nameof(galleryTypeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logger.LogInformation("Getting gallery type with ID {GalleryTypeId}", id);
            var galleryType = await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
            return galleryType == null ? null : _mapper.Map<GalleryTypeDto>(galleryType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting gallery type with ID {GalleryTypeId}", id);
            throw;
        }
    }

    public async Task<GalleryTypeDto> CreateAsync(GalleryTypeCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating gallery type {GalleryTypeName}", dto.Name);
            var galleryType = _mapper.Map<GalleryType>(dto);
            galleryType.CreatedDate = DateTime.UtcNow;
            galleryType.IsActive = true;
            galleryType.CreatedBy = "system";

            var created = await _galleryTypeRepository.AddAsync(galleryType, cancellationToken);
            return _mapper.Map<GalleryTypeDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating gallery type {GalleryTypeName}", dto.Name);
            throw;
        }
    }

    public async Task UpdateAsync(int id, GalleryTypeUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating gallery type {GalleryTypeId}", id);
            var galleryType = await _galleryTypeRepository.GetByIdAsync(id, cancellationToken);
            if (galleryType == null)
            {
                throw new InvalidOperationException($"Gallery type with ID {id} not found");
            }

            _mapper.Map(dto, galleryType);
            galleryType.ModifiedDate = DateTime.UtcNow;
            galleryType.ModifiedBy = "system";

            await _galleryTypeRepository.UpdateAsync(galleryType, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type {GalleryTypeId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting gallery type {GalleryTypeId}", id);
            await _galleryTypeRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type {GalleryTypeId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<GalleryTypeDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching gallery types with term {SearchTerm}", searchTerm);
            var galleryTypes = await _galleryTypeRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<GalleryTypeDto>>(galleryTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
