using Effortless.Api.Features.Auth.Token.Request;
using Effortless.Api.Features.Auth.Token.Response;
using Effortless.Api.Features.User;
using Effortless.Core.Extensions;
using Effortless.Core.Services.Jwt;

using RW;

namespace Effortless.Api.Features.Auth.Token;

public interface ITokenHandler
{
    Task<IResultWrapper<RefreshTokenResponse>> RenewTokensAsync(RefreshTokenRequest request);
}

public sealed class TokenHandler : ITokenHandler
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;

    public TokenHandler(
        IJwtService jwtService,
        IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }


    public async Task<IResultWrapper<RefreshTokenResponse>> RenewTokensAsync(RefreshTokenRequest request)
    {
        var expiredTokenPrinciples = _jwtService.GetPrincipalFromExpiredToken(request.Token);

        if (expiredTokenPrinciples is null)
        {
            return ResultWrapper.Failure<RefreshTokenResponse>();
        }
        
        var userPhoneNumber = expiredTokenPrinciples.GetPhoneNumber();

        var appUser = await _userRepository.GetByPhoneNumberAsync(userPhoneNumber);

        if (appUser is null)
        {
            return ResultWrapper.Failure<RefreshTokenResponse>();
        }

        if (appUser.RefreshToken != request.RefreshToken)
        {
            return ResultWrapper.Failure<RefreshTokenResponse>();
        }

        // Update appuser refresh token and refresh token expiry
        var tokenInfo = _jwtService.GetTokensAsync(appUser);

        appUser.RefreshToken = tokenInfo.RefreshToken;
        appUser.RefreshTokenExpiry = tokenInfo.RefreshTokenExpiry;

        var isUserUpdated = await _userRepository.UpdateAsync(appUser);

        if (!isUserUpdated)
        {
            return ResultWrapper.Failure<RefreshTokenResponse>();
        }
        return ResultWrapper.Success(new RefreshTokenResponse(tokenInfo.JwtToken,
            tokenInfo.RefreshToken,
            tokenInfo.RefreshTokenExpiry));
    }
}











