using Microsoft.AspNetCore.Identity;

namespace WebGallery.Domain.Entities;

/// <summary>
/// Application user entity
/// </summary>
public class ApplicationUser : IdentityUser
{
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
}
