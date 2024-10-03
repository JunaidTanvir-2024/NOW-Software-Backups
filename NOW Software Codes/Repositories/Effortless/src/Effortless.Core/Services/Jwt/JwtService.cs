using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using Effortless.Core.Domain.Entities;
using Effortless.Core.Services.TimeWrap;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Effortless.Core.Services.Jwt;
internal sealed class JwtService : IJwtService
{
    private readonly ITimeWrapService _dateTimeService;
    private readonly JwtSetting _jwtSettings;

    public JwtService(
        ITimeWrapService dateTimeService,
        IOptions<JwtSetting> jwtSettings)
    {
        _dateTimeService = dateTimeService;
        _jwtSettings = jwtSettings.Value;
    }
    public TokensInfo GetTokensAsync(UserEntity? appUser)
    {
        return new TokensInfo(
            GenerateJwtToken(appUser),
            GenerateRefreshToken(),
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays));
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(_jwtSettings.AsymmetricFiles.SecretKeyFile))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        var sigingKey = GetSecurityKeyViaFile(_jwtSettings.AsymmetricFiles.SecretKeyFile);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new ECDsaSecurityKey(sigingKey),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken)
        {
            throw new Exception("identity.invalidtoken");
        }

        if (!jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.EcdsaSha512,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new Exception("identity.invalidtoken");
        }

        return principal;
    }

    private string GenerateJwtToken(UserEntity? appUser)
    {
        var userClaims = GenerateUserClaims(appUser);

        var sigingKey = GetSecurityKeyViaFile(_jwtSettings.AsymmetricFiles.SecretKeyFile!);

        var securityToken = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            NotBefore = DateTime.Now,
            Expires = _dateTimeService.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes),
            IssuedAt = DateTime.Now,
            Claims = userClaims,
            SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(sigingKey),
                    SecurityAlgorithms.EcdsaSha512)
        };
        return new JsonWebTokenHandler().CreateToken(securityToken);
    }

    private static Dictionary<string, object> GenerateUserClaims(UserEntity? appUser)
    {
        ArgumentNullException.ThrowIfNull(appUser);

        return new Dictionary<string, object>
        {
            { ClaimTypes.Sid, appUser.Id },
            { ClaimTypes.Name, appUser.FirstName ?? string.Empty },
            { ClaimTypes.Surname, appUser.LastName ?? string.Empty },
            { ClaimTypes.Email, appUser.Email ?? string.Empty },
            { ClaimTypes.MobilePhone, appUser.PhoneNumber ?? string.Empty },
        };
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        return refreshToken;
    }

    public ECDsa GetSecurityKeyViaFile(string filePath)
    {
        var eccPem = File.ReadAllText(filePath);
        var key = ECDsa.Create();
        key.ImportFromPem(eccPem);
        return key;
    }
}
