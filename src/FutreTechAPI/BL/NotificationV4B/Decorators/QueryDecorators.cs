using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV4B.Decorators
{
    public class LoggingQueryDecorator<TRequest, TResponse> : IQueryHandler<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
        private readonly IQueryHandler<TRequest, TResponse> request;
        private readonly ILogger logger;

        public LoggingQueryDecorator(IQueryHandler<TRequest, TResponse> request, ILogger logger)
        {
            this.request = request;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest src)
        {
            var result = await request.Handle(src);
            logger.Write("Generic logging");
            return result;
        }
    }

    public class ValidationQueryDecorator<TRequest, TResponse> : IQueryHandler<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
        private readonly IQueryHandler<TRequest, TResponse> request;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationQueryDecorator(IQueryHandler<TRequest, TResponse> request, IEnumerable<IValidator<TRequest>> validators)
        {
            this.request = request;
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest src)
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

    public class RetryQueryDecorator<TRequest, TResponse> : IQueryHandler<TRequest, TResponse>
        where TRequest : IQuery<TResponse>
    {
        private readonly IQueryHandler<TRequest, TResponse> request;
        private readonly ILogger _logger;
        private readonly int nrRetry;

        public RetryQueryDecorator(IQueryHandler<TRequest, TResponse> request, ILogger logger, int nrRetry = 5)
        {
            this.request = request;
            _logger = logger;
            this.nrRetry = nrRetry;
        }

        public async Task<TResponse> Handle(TRequest src)
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
}
