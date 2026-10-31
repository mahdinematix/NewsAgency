using FluentValidation;
using MediatR;
using NewsAgency.Application.Exceptions;

namespace NewsAgency.Application.Common;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();

        foreach (var validator in _validators)
        {
            var context = new ValidationContext<TRequest>(request);

            var result = await validator.ValidateAsync(
                context,
                cancellationToken);

            errors.AddRange(result.Errors.Select(error =>
                new ValidationError(
                    error.PropertyName,
                    error.ErrorMessage)));
        }

        if (errors.Count > 0)
        {
            throw new Exceptions.ValidationException(errors);
        }

        return await next(cancellationToken);
    }
}