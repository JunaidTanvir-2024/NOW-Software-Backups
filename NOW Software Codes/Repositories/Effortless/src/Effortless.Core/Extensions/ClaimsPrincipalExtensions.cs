using System.Security.Claims;

namespace Effortless.Core.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email);
    }
    public static string? GetPhoneNumber(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.MobilePhone);
    }
}
