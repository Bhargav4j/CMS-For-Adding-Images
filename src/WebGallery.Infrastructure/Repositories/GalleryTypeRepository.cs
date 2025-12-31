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
        return await _context.GalleryTypes
            .AsNoTracking()
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<GalleryType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.GalleryTypes
            .AsNoTracking()
            .Include(g => g.Pictures)
            .FirstOrDefaultAsync(g => g.Id == id && g.IsActive, cancellationToken);
    }

    public async Task AddAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        await _context.GalleryTypes.AddAsync(galleryType, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(GalleryType galleryType, CancellationToken cancellationToken = default)
    {
        _context.GalleryTypes.Update(galleryType);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var galleryType = await _context.GalleryTypes.FindAsync(new object[] { id }, cancellationToken);
        if (galleryType != null)
        {
            galleryType.IsActive = false;
            galleryType.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.GalleryTypes.AnyAsync(g => g.Id == id && g.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<GalleryType>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.GalleryTypes
            .AsNoTracking()
            .Where(g => g.IsActive && (g.Name.Contains(searchTerm) || g.Description.Contains(searchTerm)))
            .OrderBy(g => g.Name)
            .ToListAsync(cancellationToken);
    }
}
