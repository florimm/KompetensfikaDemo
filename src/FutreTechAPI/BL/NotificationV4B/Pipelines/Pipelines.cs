using System;
using FluentValidation;
using FutreTechAPI.BL.NotificationV4B.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace FutreTechAPI.BL.NotificationV4B.Pipelines
{
    public delegate ICommandHandler<TCommand> CommandPipeline<TCommand>(object obj, IServiceProvider serviceProvider);

    public delegate IQueryHandler<TQuery, TResponse> QueryPipeline<TQuery, TResponse>(object obj, IServiceProvider serviceProvider)
        where TQuery : IQuery<TResponse>;

    public class Command
    {
        public class Pipelines
        {
            public static ICommandHandler<TCommand> Default<TCommand>(object handler, IServiceProvider provider)
            {
                var logger = provider.GetService<ILogger>();
                var validations = provider.GetServices<IValidator<TCommand>>();
                var transactionDecorator = new TransactionDecorator<TCommand>((ICommandHandler<TCommand>)handler);
                var loggingDecorator = new LoggingDecorator<TCommand>(transactionDecorator, logger);
                var validationDecorator = new ValidationDecorator<TCommand>(loggingDecorator, validations);
                return validationDecorator; // order is revers for decorators
            }

            public static ICommandHandler<TCommand> DefaultWithRetry<TCommand>(object handler, IServiceProvider provider)
            {
                var logger = provider.GetService<ILogger>();
                var cache = provider.GetService<ICache>();
                var validations = provider.GetServices<IValidator<TCommand>>();

                var transactionDecorator = new TransactionDecorator<TCommand>((ICommandHandler<TCommand>)handler);
                var loggingDecorator = new LoggingDecorator<TCommand>(transactionDecorator, logger);
                var validationDecorator = new ValidationDecorator<TCommand>(loggingDecorator, validations);
                var retryDecorator = new RetryDecorator<TCommand>(validationDecorator, logger, nrRetry: 3);
                var cacheDecorator = new CacheDecorator<TCommand>(retryDecorator, cache);
                return cacheDecorator; // order is revers for decorators
            }
        }
    }

    public class Query
    {
        public class Pipelines
        {
            public static IQueryHandler<TQuery, TResponse> Default<TQuery, TResponse>(object handler, IServiceProvider provider) where TQuery : IQuery<TResponse>
            {
                var logger = provider.GetService<ILogger>();
                var validations = provider.GetServices<IValidator<TQuery>>();
                var loggingDecorator = new LoggingQueryDecorator<TQuery, TResponse>((IQueryHandler<TQuery, TResponse>)handler, logger);
                var validationDecorator = new ValidationQueryDecorator<TQuery, TResponse>(loggingDecorator, validations);
                var retryDecorator = new RetryQueryDecorator<TQuery, TResponse>(validationDecorator, logger);
                return retryDecorator; // order is revers for decorators
            }
        }
    }
}
