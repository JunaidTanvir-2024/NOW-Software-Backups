using Effortless.Api.Features.User;
using Effortless.Core.Domain.Definitions;
using Effortless.Core.Services.Identity;

using Effortless.Core.Services.Jwt;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.Localization;

using RW;

namespace Effortless.Api.Features.Auth;
public sealed class Login
{
    #region Query
    public sealed record Query : IRequest<IResultWrapper>
    {
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required IFormFile ProfilePhoto { get; set; }
    }
    #endregion

    #region Validator
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.PhoneNumber).NotNull().NotEmpty();
            RuleFor(p => p.Password).NotNull().NotEmpty().MinimumLength(6);
        }
    }
    #endregion

    #region Result
    public sealed record Response
    {
        public required string Name { get; set; }
        public required string PhoneNumber { get; set; }
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
    #endregion

    #region Handler
    internal sealed class Handler(
        IAuthService authService,
        IJwtService jwtService,
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        IStringLocalizer<Query> localizer) : IRequestHandler<Query, IResultWrapper>
    {
        private readonly IAuthService _authService = authService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IStringLocalizer _localizer = localizer;

        public async Task<IResultWrapper> Handle(Query request, CancellationToken cancellationToken)
        {
            var appUser = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);

            if (appUser is null)
            {
                return ResultWrapper.Failure<Response>(
                    _localizer[AppConstant.Status.Key.AccountNotExist], AppConstant.Status.Code.AccountNotExist);
            }

            var loggedInResult = await _authService.LoginAsync(appUser, request.Password);

            if (loggedInResult?.Succeeded is false)
            {
                return ResultWrapper.Failure<Response>(
                    _localizer[AppConstant.Status.Key.InvalidCredentials],AppConstant.Status.Code.InvalidCredentials);
            }

            if (loggedInResult?.IsLockedOut is true)
            {
                return ResultWrapper.Failure<Response>(
                   _localizer[AppConstant.Status.Key.UserBlocked], AppConstant.Status.Code.LockedOut);
            }

            var tokenInfo = _jwtService.GetTokensAsync(appUser);

            appUser.RefreshToken = tokenInfo.RefreshToken;
            appUser.RefreshTokenExpiry = tokenInfo.RefreshTokenExpiry;

            await _userRepository.UpdateAsync(appUser);

            _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", tokenInfo.RefreshToken);

            return ResultWrapper.Success(
                new Response()
                {
                    JwtToken = tokenInfo.JwtToken,
                    Name = appUser.FirstName!,
                    PhoneNumber = appUser.PhoneNumber!,
                    RefreshToken = tokenInfo.RefreshToken!,
                    RefreshTokenExpiry = tokenInfo.RefreshTokenExpiry,
                });
        }
    }
    #endregion
}
