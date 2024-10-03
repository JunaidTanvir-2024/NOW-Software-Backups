namespace Effortless.Core.Services.Jwt;
public record TokensInfo(string JwtToken, string RefreshToken, DateTime RefreshTokenExpiry);
