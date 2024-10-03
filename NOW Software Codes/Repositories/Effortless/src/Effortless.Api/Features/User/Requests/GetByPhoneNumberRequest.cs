using FluentValidation;

namespace Effortless.Api.Features.User.Requests;
public class GetByPhoneNumberRequest
{
    public string PhoneNumber { get; set; } = default!;
}

public class GetUserByPhoneNumberRequestValidator : AbstractValidator<GetByPhoneNumberRequest>
{
    public GetUserByPhoneNumberRequestValidator()
    {
        RuleFor(p => p.PhoneNumber).NotNull().NotEmpty();
    }
}
