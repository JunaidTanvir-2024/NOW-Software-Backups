using FluentValidation;

namespace Effortless.Api.Features.Auth.Token.Request;

public record RefreshTokenRequest(string Token, string RefreshToken);

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(p => p.Token).NotEmpty().NotNull();
        RuleFor(p => p.RefreshToken).NotEmpty().NotNull();
    }
}
