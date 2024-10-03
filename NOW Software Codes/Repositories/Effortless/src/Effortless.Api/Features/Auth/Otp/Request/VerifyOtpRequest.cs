using FluentValidation;

namespace Effortless.Api.Features.Auth.Otp.Request;
public class VerifyOtpRequest
{
    public int Type { get; set; }
    public long Code { get; set; }
}

public class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
{
    public VerifyOtpRequestValidator()
    {
        RuleFor(p => p.Code).NotNull().NotEmpty();
        RuleFor(p => p.Type).NotNull().NotEmpty().GreaterThanOrEqualTo(1);
    }
}
