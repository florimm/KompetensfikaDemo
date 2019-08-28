using FutreTechAPI.BL.NotificationV4B;
using FutreTechAPI.BL.NotificationV4B.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace FutreTechAPI.Helpers
{
    public static class DIExtensions
    {
        public static IServiceCollection RegisterCommandWithPipeline<TCommand, THandler>(this IServiceCollection services, CommandPipeline<TCommand> pipeline)
        {
            services.AddTransient(typeof(ICommandHandler<TCommand>), typeof(THandler));
            services.Decorate(typeof(ICommandHandler<TCommand>), (obj, provider) => pipeline(obj, provider));
            return services;
        }

        public static IServiceCollection RegisterQueryWithPipeline<TQuery, THandler, TResponse>(this IServiceCollection services, QueryPipeline<TQuery, TResponse> pipeline)
            where TQuery : IQuery<TResponse>
        {
            services.AddTransient(typeof(IQueryHandler<TQuery, TResponse>), typeof(THandler));
            services.Decorate(typeof(IQueryHandler<TQuery, TResponse>), (obj, provider) => pipeline(obj, provider));
            return services;
        }
    }
}
