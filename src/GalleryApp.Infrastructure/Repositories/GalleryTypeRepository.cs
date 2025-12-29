using GalleryApp.Domain.Entities;
using GalleryApp.Domain.Interfaces.Repositories;
using GalleryApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GalleryApp.Infrastructure.Repositories;

public class GalleryTypeRepository : IGalleryTypeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GalleryTypeRepository> _logger;

    public GalleryTypeRepository(ApplicationDbContext context, ILogger<GalleryTypeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GalleryType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.GalleryTypes
                .Where(g => g.IsActive)
                .AsNoTracking()
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
            _logger.LogError(ex, "Error retrieving gallery type with ID: {GalleryTypeId}", id);
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
            _logger.LogError(ex, "Error deleting gallery type with ID: {GalleryTypeId}", id);
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
            _logger.LogError(ex, "Error checking if gallery type exists with ID: {GalleryTypeId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.GalleryTypes
                .Where(g => g.IsActive && (g.Name.Contains(searchTerm) || (g.Description != null && g.Description.Contains(searchTerm))))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching gallery types with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
