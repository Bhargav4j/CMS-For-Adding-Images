using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Infrastructure.Data;

namespace WebGallery.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Picture entity
/// </summary>
public class PictureRepository : IPictureRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PictureRepository> _logger;

    public PictureRepository(ApplicationDbContext context, ILogger<PictureRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Picture>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Pictures
                .Include(p => p.GalleryType)
                .Where(p => p.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
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
            return await _context.Pictures
                .Include(p => p.GalleryType)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving picture with ID {PictureId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> GetByGalleryTypeAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Pictures
                .Include(p => p.GalleryType)
                .Where(p => p.GalleryTypeId == galleryTypeId && p.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pictures for gallery type {GalleryTypeId}", galleryTypeId);
            throw;
        }
    }

    public async Task<Picture> AddAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Pictures.AddAsync(picture, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return picture;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding picture");
            throw;
        }
    }

    public async Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Pictures.Update(picture);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating picture with ID {PictureId}", picture.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var picture = await GetByIdAsync(id, cancellationToken);
            if (picture != null)
            {
                picture.IsActive = false;
                await UpdateAsync(picture, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture with ID {PictureId}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Pictures
                .AnyAsync(p => p.Id == id && p.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if picture exists with ID {PictureId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Pictures
                .Include(p => p.GalleryType)
                .Where(p => p.IsActive &&
                    (p.Name.Contains(searchTerm) ||
                     (p.Description != null && p.Description.Contains(searchTerm))))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term {SearchTerm}", searchTerm);
            throw;
        }
    }
}
