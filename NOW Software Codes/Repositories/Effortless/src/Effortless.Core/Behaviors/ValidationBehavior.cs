using FluentValidation;

using MediatR;

using RW;

namespace Effortless.Core.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, IResultWrapper>
    where TRequest : IRequest<IResultWrapper>
{
    public async Task<IResultWrapper> Handle(TRequest request, RequestHandlerDelegate<IResultWrapper> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationFailures = validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (validationFailures.Count is not 0)
        {
            return ResultWrapper.Failure(validationFailures);
        }

        return await next();
    }
}
