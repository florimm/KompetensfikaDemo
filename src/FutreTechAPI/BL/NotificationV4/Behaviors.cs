using FluentValidation;
using FutreTechAPI.Common;
using MediatR;
using MediatR.Pipeline;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace FutreTechAPI.BL.NotificationV4
{
    #region Behaviors

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger logger;

        public LoggingBehavior(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            logger.Write($"Handling {typeof(TRequest).Name}");
            var response = await next();
            logger.Write($"Handled {typeof(TResponse).Name}");
            return response;
        }
    }

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }

    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var result = await next();
                scope.Complete();
                return result;
            }
        }
    }

    public class RetryUnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;

        public RetryUnitOfWorkBehavior(ILogger logger)
        {
            _logger = logger;
        }

        // inject IUnitOfWork (DBContext) or something that track changes
        public Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    return next();
                }
                catch (SqlException ex)
                {
                    _logger.Write(ex.Message);// log
                    if (retryCount >= 5)
                        throw;

                    //Reset things and retry again
                    retryCount++;
                }
            }
        }
    }

    #endregion
}
