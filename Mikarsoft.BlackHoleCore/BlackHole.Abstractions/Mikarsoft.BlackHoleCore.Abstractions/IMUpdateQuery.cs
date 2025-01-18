using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    public interface IMUpdateQuery<T, G> where T : BHEntityAI<T, G>, new() where G : struct, IBHStruct
    {
        IMUpdate<T> ById(T item);

        IMUpdate<Dto> ById<Dto>(Dto item) where Dto : BHDto<G>;

        IMUpdate<T> ById(List<T> item);

        IMUpdate<Dto> ById<Dto>(List<Dto> item) where Dto : BHDto<G>;
    }

    public interface IMUpdateQuery<T> where T : BHEntity<T>, new()
    {
        IMUpdate<T> Where(Expression<Func<T, bool>> predicate , T item);
    }

    public interface IMUpdateMapper<T, Dto> : IMUpdate<Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMUpdate<Dto> Columns(Action<BHUpdateMappingConfig<T, Dto>> mapping);
    }

    public interface IMUpdateMapper<T> : IMUpdate<T> where T : BHEntity<T>, new()
    {
        IMUpdate<T> Columns(Action<BHUpdateMappingConfig<T>> mapping);
    }

    public interface IMUpdate<T> where T : class
    {
        Task<bool> ExecuteAsync();

        bool Execute();
    }
}
