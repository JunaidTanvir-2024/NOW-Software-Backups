using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Effortless.Core.Extensions;
public static class IdentityExtensions
{
    public static List<string> GetErrors(this IdentityResult result, IStringLocalizer t) =>
        result.Errors.Select(e => t[e.Description].ToString()).ToList();
}
