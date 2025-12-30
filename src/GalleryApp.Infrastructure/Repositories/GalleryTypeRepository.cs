using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GalleryApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for GalleryType entity
/// </summary>
public class GalleryTypeRepository : IGalleryTypeRepository
{
    private readonly GalleryDbContext _context;
    private readonly ILogger<GalleryTypeRepository> _logger;

    public GalleryTypeRepository(GalleryDbContext context, ILogger<GalleryTypeRepository> logger)
    {
        _context = context;
        _logger = logger;
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
            _logger.LogError(ex, "Error retrieving all gallery types from database");
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
            _logger.LogError(ex, "Error retrieving gallery type by ID: {Id}", id);
            throw;
        }
    }

    public async Task<GalleryType> AddAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.GalleryTypes.AddAsync(galleryType, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return galleryType;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding gallery type to database");
            throw;
        }
    }

    public async Task UpdateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.GalleryTypes.Update(galleryType);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating gallery type in database");
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var galleryType = await GetByIdAsync(id, cancellationToken);
            if (galleryType != null)
            {
                galleryType.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting gallery type from database");
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
            _logger.LogError(ex, "Error checking if gallery type exists");
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
                .Where(g => g.IsActive &&
                    (g.Name.Contains(searchTerm) ||
                     (g.Description != null && g.Description.Contains(searchTerm))))
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
