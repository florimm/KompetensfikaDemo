using FutreTechAPI.Common;
using System.Threading.Tasks;

namespace FutreTechAPI.BL.NotificationV4B
{
    public interface ICommand
    {

    }
    public interface ICommandHandler<T>
    {
        Task<Result> Handle(T src);
    }

    public interface IQuery<TResult>
    {
    }

    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }

    public interface ICacheble
    {
        (string, string) CacheKeyVal();
    }
}
