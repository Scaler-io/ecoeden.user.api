using Ecoeden.User.Application.Contracts.Data;
using Ecoeden.User.Application.Extensions;
using MediatR;

namespace Ecoeden.User.Application.Behaviors;

public class DbTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDbTranscation _dbTransaction;
    private ILogger _logger;

    public DbTransactionBehavior(IDbTranscation dbTransaction, ILogger logger)
    {
        _dbTransaction = dbTransaction;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, 
        RequestHandlerDelegate<TResponse> next)
    {
        if(request is ISkipPiplineBehavior)
        {
            _logger.Here().Information("Skipping transaction for request {TRequest}", typeof(TRequest).Name);
            return await next();
        }

        _logger.Here().Information("Starting transaction for the request {TRequest}", typeof(TRequest).Name);
        _dbTransaction.BeginTransaction();

        var response = await next();

        _logger.Here().Information("Committing transaction for the request {TRequest}", typeof(TRequest).Name);
        _dbTransaction.CommitTransaction();

        return response;
    }
}
