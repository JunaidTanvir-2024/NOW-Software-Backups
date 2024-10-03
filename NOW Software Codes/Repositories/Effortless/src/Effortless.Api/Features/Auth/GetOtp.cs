using FluentValidation;

namespace Effortless.Api.Features.Auth;

public sealed class GetOtp
{
    public record class Query
    {
        public required string PhoneNumber { get; set; }
        public required int Type { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(p => p.PhoneNumber).NotNull().NotEmpty();
            RuleFor(p => p.Type).NotNull().NotEmpty();
        }
    }

    public record class Response
    {
        public required long Code { get; set; }
        public required string UserId { get; set; }
        public required string PhoneNumber { get; set; }
        public required DateTimeOffset ExpiryTime { get; set; }
        public required int Type { get; set; }
    }

}
