using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FluentValidation;
using FutreTechAPI.Common;

namespace FutreTechAPI.BL.NotificationV4B
{
    public class LoggingDecorator<TRequest> : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> request;
        private readonly ILogger logger;

        public LoggingDecorator(ICommandHandler<TRequest> request, ILogger logger)
        {
            this.request = request;
            this.logger = logger;
        }

        public async Task<Result> Handle(TRequest src)
        {
            var result = await request.Handle(src);
            logger.Write("Generic logging");
            return result;
        }
    }

    public class TransactionDecorator<TRequest> : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> request;

        public TransactionDecorator(ICommandHandler<TRequest> request)
        {
            this.request = request;
        }

        public async Task<Result> Handle(TRequest src)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var result = await request.Handle(src);
                scope.Complete();
                return result;
            }
        }
    }

    public class ValidationDecorator<TRequest> : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> request;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationDecorator(ICommandHandler<TRequest> request, IEnumerable<IValidator<TRequest>> validators)
        {
            this.request = request;
            _validators = validators;
        }

        public async Task<Result> Handle(TRequest src)
        {
            var context = new ValidationContext(src);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await request.Handle(src);
        }
    }

    public class RetryDecorator<TRequest> : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> request;
        private readonly ILogger _logger;
        private readonly int nrRetry;

        public RetryDecorator(ICommandHandler<TRequest> request, ILogger logger, int nrRetry = 5)
        {
            this.request = request;
            _logger = logger;
            this.nrRetry = nrRetry;
        }

        public async Task<Result> Handle(TRequest src)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    return await request.Handle(src);
                }
                catch (Exception ex)
                {
                    _logger.Write(ex.Message);// log
                    if (retryCount >= nrRetry)
                        throw;

                    //Reset things and retry again
                    retryCount++;
                }
            }
        }
    }

    public class CacheDecorator<TRequest> : ICommandHandler<TRequest>
    {
        private readonly ICommandHandler<TRequest> request;
        private readonly ICache cache;

        public CacheDecorator(ICommandHandler<TRequest> request, ICache cache)
        {
            this.request = request;
            this.cache = cache;
        }

        public async Task<Result> Handle(TRequest src)
        {
            var cacheData = src as ICacheble;
            if (cacheData != null)
            {
                (string key, string val) = cacheData.CacheKeyVal();
                if (cache.Get<string>(key) == val)
                {
                    return Result.Ok(cache.Get<string>(key));
                }
                else
                {
                    var result = await request.Handle(src);
                    if (result.IsSuccess)
                    {
                        cache.Add(key, val);
                    }
                    return result;
                }
            }
            return await request.Handle(src);
        }
    }
}
