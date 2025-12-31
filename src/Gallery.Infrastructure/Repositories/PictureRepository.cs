using Gallery.Domain.Entities;
using Gallery.Domain.Interfaces.Repositories;
using Gallery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gallery.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Picture entity
/// </summary>
public class PictureRepository : IPictureRepository
{
    private readonly GalleryDbContext _context;
    private readonly ILogger<PictureRepository> _logger;

    public PictureRepository(GalleryDbContext context, ILogger<PictureRepository> logger)
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
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving picture with ID: {Id}", id);
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
            _logger.LogError(ex, "Error retrieving pictures for gallery type: {GalleryTypeId}", galleryTypeId);
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
            _logger.LogError(ex, "Error adding picture: {Name}", picture.Name);
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
            _logger.LogError(ex, "Error updating picture: {Id}", picture.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var picture = await _context.Pictures.FindAsync(new object[] { id }, cancellationToken);
            if (picture != null)
            {
                picture.IsActive = false;
                picture.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting picture with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Pictures.AnyAsync(p => p.Id == id && p.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if picture exists: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync(cancellationToken);
            }

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
            _logger.LogError(ex, "Error searching pictures with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
