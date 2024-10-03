using FluentValidation;

namespace Effortless.Api.Features.Auth.Password.Request;
public class SetNewPasswordRequest
{
    public string PhoneNumber { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}
public class SetNewPasswordRequestValidator : AbstractValidator<SetNewPasswordRequest>
{
    public SetNewPasswordRequestValidator()
    {
        RuleFor(p => p.PhoneNumber).NotNull().NotEmpty();
        RuleFor(p => p.Password).NotNull().NotEmpty().MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords should match");
    }
}
