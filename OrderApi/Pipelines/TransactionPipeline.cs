using MediatR;
using OrderApi.Database;
using Shared.Abstracts;

namespace OrderApi.Pipelines;

public sealed class TransactionPipeline<TRequest, TResponse>
    (OrderDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (!typeof(TRequest).Name.EndsWith("Command"))
            return await next(cancellationToken);
        
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch (Exception ex) { 
            await transaction.RollbackAsync(cancellationToken);

            throw new Exception(ex.Message);
        }
    }
}
