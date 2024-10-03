using System.Collections.Immutable;

using FluentValidation;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using RW;

namespace Effortless.Core.Filters;
public class FluentValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? request = default;

        // Determining the position of request argument.
        for (int i = context.Arguments.Count - 1; i >= 0; i--)
        {
            if (context.Arguments[i] is T)
            {
                request = context.GetArgument<T>(i);
                break;
            }
        }
        IValidator<T>? validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is not null && request is not null)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error =>
                {
                    return new { message = error.ErrorMessage, code = error.ErrorCode };

                }).ToImmutableList();

                return Results.BadRequest(ResultWrapper.Failure(errors));
            }

        }
        // Otherwise invoke the next filter in the pipeline
        return await next.Invoke(context);
    }
}
