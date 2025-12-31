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
        return await _context.Pictures
            .AsNoTracking()
            .Include(p => p.GalleryType)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Picture?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pictures
            .AsNoTracking()
            .Include(p => p.GalleryType)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Picture>> GetByGalleryTypeIdAsync(int galleryTypeId, CancellationToken cancellationToken = default)
    {
        return await _context.Pictures
            .AsNoTracking()
            .Include(p => p.GalleryType)
            .Where(p => p.GalleryTypeId == galleryTypeId && p.IsActive)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        await _context.Pictures.AddAsync(picture, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Picture picture, CancellationToken cancellationToken = default)
    {
        _context.Pictures.Update(picture);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var picture = await _context.Pictures.FindAsync(new object[] { id }, cancellationToken);
        if (picture != null)
        {
            picture.IsActive = false;
            picture.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pictures.AnyAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<IEnumerable<Picture>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Pictures
            .AsNoTracking()
            .Include(p => p.GalleryType)
            .Where(p => p.IsActive && (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
