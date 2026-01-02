using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebGallery.Domain.Entities;
using WebGallery.Domain.Interfaces.Repositories;
using WebGallery.Infrastructure.Data;

namespace WebGallery.Infrastructure.Repositories;

public class PictureRepository : IPictureRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PictureRepository> _logger;

    public PictureRepository(ApplicationDbContext context, ILogger<PictureRepository> logger)
    {
        _context = context;
        _logger = logger;
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
            _logger.LogError(ex, "Error retrieving all pictures from database");
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
            _logger.LogError(ex, "Error retrieving picture with ID {PictureId} from database", id);
            throw;
        }
    }

    public async Task<IEnumerable<Picture>> GetByGalleryIdAsync(int galleryId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Pictures
                .Include(p => p.GalleryType)
                .Where(p => p.GalleryId == galleryId && p.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pictures for gallery ID {GalleryId} from database", galleryId);
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
            _logger.LogError(ex, "Error adding picture to database");
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
            _logger.LogError(ex, "Error updating picture with ID {PictureId} in database", picture.Id);
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
            _logger.LogError(ex, "Error deleting picture with ID {PictureId} from database", id);
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
            _logger.LogError(ex, "Error checking if picture with ID {PictureId} exists in database", id);
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
                     p.Description.Contains(searchTerm)))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching pictures with term {SearchTerm} in database", searchTerm);
            throw;
        }
    }
}
