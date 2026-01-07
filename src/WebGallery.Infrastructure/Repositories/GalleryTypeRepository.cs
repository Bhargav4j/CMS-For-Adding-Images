using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Infrastructure.Data;

namespace WebGallery.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for GalleryType entity
/// </summary>
public class GalleryTypeRepository : IGalleryTypeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GalleryTypeRepository> _logger;

    public GalleryTypeRepository(ApplicationDbContext context, ILogger<GalleryTypeRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.GalleryTypes
                .AsNoTracking()
                .Include(g => g.Pictures)
                .Where(g => g.IsActive)
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all gallery types");
            throw;
        }
    }

    public async Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.GalleryTypes
                .Include(g => g.Pictures)
                .FirstOrDefaultAsync(g => g.Id == id && g.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<GalleryType> AddAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            galleryType.CreatedDate = DateTime.UtcNow;
            galleryType.IsActive = true;
            _context.GalleryTypes.Add(galleryType);
            await _context.SaveChangesAsync(cancellationToken);
            return galleryType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding gallery type: {Name}", galleryType.Name);
            throw;
        }
    }

    public async Task UpdateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            galleryType.ModifiedDate = DateTime.UtcNow;
            _context.GalleryTypes.Update(galleryType);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type with ID: {Id}", galleryType.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var galleryType = await _context.GalleryTypes.FindAsync(new object[] { id }, cancellationToken);
            if (galleryType != null)
            {
                galleryType.IsActive = false;
                galleryType.ModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.GalleryTypes.AnyAsync(g => g.Id == id && g.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if gallery type exists with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.GalleryTypes
                .AsNoTracking()
                .Include(g => g.Pictures)
                .Where(g => g.IsActive && (g.Name.Contains(searchTerm) || (g.Description != null && g.Description.Contains(searchTerm))))
                .OrderBy(g => g.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
