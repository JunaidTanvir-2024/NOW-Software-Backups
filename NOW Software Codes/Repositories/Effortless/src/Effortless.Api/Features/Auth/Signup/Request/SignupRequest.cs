using FluentValidation;

namespace Effortless.Api.Features.Auth.Signup.Request;
public class SignupRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

internal class SignupRequestValidator : AbstractValidator<SignupRequest>
{
    public SignupRequestValidator()
    {
        RuleFor(p => p.UserEmail).NotNull().NotEmpty();
        RuleFor(p => p.PhoneNumber).NotNull().NotEmpty();
        RuleFor(p => p.FirstName).NotNull().NotEmpty();
        RuleFor(p => p.LastName).NotNull().NotEmpty();
        RuleFor(p => p.UserName).NotNull().NotEmpty();
        RuleFor(p => p.Password).NotNull().NotEmpty().MinimumLength(6);
        RuleFor(p => p.ConfirmPassword).Equal(p => p.Password);
    }
}