using System.Security.Claims;
using System.Security.Cryptography;

using Effortless.Core.Domain.Entities;

namespace Effortless.Core.Services.Jwt;

public interface IJwtService
{
    TokensInfo GetTokensAsync(UserEntity? appUser);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    ECDsa GetSecurityKeyViaFile(string filePath);
}
