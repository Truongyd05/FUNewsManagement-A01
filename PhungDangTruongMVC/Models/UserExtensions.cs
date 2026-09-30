using System.Security.Claims;

namespace PhungDangTruongMVC.Models;

public static class UserExtensions
{
    public static short GetAccountId(this ClaimsPrincipal user)
        => short.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (short)0;

    public static string? GetRole(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Role);
}
