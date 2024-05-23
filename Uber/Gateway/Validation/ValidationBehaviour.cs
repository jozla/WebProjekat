using FluentValidation;
using MediatR;

namespace Gateway.Validation;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validator;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator.Any())
        {
            var validationContext = new ValidationContext<TRequest>(request);
            var result = await Task.WhenAll(_validator.Select(v => v.ValidateAsync(validationContext, cancellationToken)));
            var errors = result.SelectMany(result => result.Errors).Where(error => error != null).ToList();

            if (errors.Count() > 0)
            {
                throw new ValidationException(errors);
            }
        }

        return await next();
    }
}
