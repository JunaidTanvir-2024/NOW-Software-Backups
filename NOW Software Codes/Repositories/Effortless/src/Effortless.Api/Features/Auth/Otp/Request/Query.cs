using FluentValidation;

namespace Effortless.Api.Features.Auth.Otp.Request;
public class Query
{
    public string PhoneNumber { get; set; } = null!;
    public int Type { get; set; }
}

public class Validator : AbstractValidator<Query>
{
    public Validator()
    {
        RuleFor(p => p.PhoneNumber).NotNull().NotEmpty();
        RuleFor(p => p.Type).NotNull().NotEmpty();
    }
}
