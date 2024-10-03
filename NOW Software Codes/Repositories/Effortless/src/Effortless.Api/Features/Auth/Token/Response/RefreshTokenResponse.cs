namespace Effortless.Api.Features.Auth.Token.Response;
public record RefreshTokenResponse(string JwtToken, string RefreshToken, DateTime RefreshTokenExpiry);
