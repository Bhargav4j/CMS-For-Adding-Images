using System.Security.Claims;

namespace Gallery.Web.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal (User)
/// </summary>
public static class UserExtensions
{
    public static bool IsInAuthenticatedRole(this ClaimsPrincipal user, string role)
    {
        if (user == null || !user.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        return user.IsInRole(role);
    }
}
