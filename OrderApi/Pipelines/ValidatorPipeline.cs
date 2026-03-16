using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace OrderApi.Pipelines;

public sealed class ValidatorPipeline<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        ValidationContext<TRequest> context = new (request);

        IEnumerable<ValidationResult> results = await Task.WhenAll(
            validators.Select(x => x.ValidateAsync(context, cancellationToken))
            );

        var failures = results.SelectMany(x => x.Errors).Where(x => x is not null);

        if (failures.Any())
        {
            var errors = failures.Select(x =>x.ErrorMessage);
            throw new ValidationException(string.Join("\n", errors));
        }

        return await next(cancellationToken);
    }
}
