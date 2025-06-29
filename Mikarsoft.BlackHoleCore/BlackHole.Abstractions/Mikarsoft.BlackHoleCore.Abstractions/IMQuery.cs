

using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    #region SelectQuery
    public interface IMQuery<T> : IMIncludeBase<T> where T : BHEntity<T>, new()
    {
        bool Any();

        bool Any(Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync();

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        int Count();

        int Count(Expression<Func<T, bool>> predicate);

        Task<int> CountAsync();

        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }

    public interface IMQuery<T, Dto> : IMIncludeBase<T, Dto> where T : BHEntity<T> , new() where Dto : class
    {
        IPreJoin<Dto, T, TOther> InnerJoin<TOther>() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, T, TOther> OuterJoin<TOther>() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, T, TOther> LeftJoin<TOther>() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, T, TOther> RightJoin<TOther>() where TOther : BHEntity<TOther>, new();
    }

    public interface IMIncludeBase<T> : IMSearchQuery<T> where T : BHEntity<T>, new()
    {
        IMThenInclude<T, G> Include<G, H>(Func<T, BHCollection<G, H>> predicate) where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;
        IMThenInclude<T, G> Include<G, H>(Func<T, BHItem<G, H>> predicate) where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;
    }

    public interface IMIncludeBase<T, Dto> : IMSearchQuery<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMThenInclude<T, G, Dto> Include<G, H>(Func<T, BHCollection<G, H>> predicate) where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;
        IMThenInclude<T, G, Dto> Include<G, H>(Func<T, BHItem<G, H>> predicate) where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;
    }

    public interface IMThenInclude<T, G> : IMIncludeBase<T> where T :BHEntity<T>, new() where G : BHEntity<G>, new()
    {
        IMThenInclude<T, J> ThenInclude<J, H>(Func<G, BHCollection<J, H>> predicate) where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;
        IMThenInclude<T, J> ThenInclude<J, H>(Func<G, BHItem<J, H>> predicate) where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;
    }

    public interface IMThenInclude<T, G, Dto> : IMIncludeBase<T, Dto> where T : BHEntity<T>, new() where G : BHEntity<G>, new() where Dto : class
    {
        IMThenInclude<T, J, Dto> ThenInclude<J, H>(Func<G, BHCollection<J, H>> predicate) where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;
        IMThenInclude<T, J, Dto> ThenInclude<J, H>(Func<G, BHItem<J, H>> predicate) where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;
    }

    public interface IMSearchQuery<T> : IMGroupBy<T> where T : BHEntity<T>, new()
    {
        IMGroupBy<T> Where(Expression<Func<T, bool>> predicate);
    }

    public interface IMGroupBy<T> : IMOrderBy<T> where T : class
    {
        IMEnumerable<IMGroupBy<G, T>, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);
    }

    public interface IMGroupBy<T, G>
    {
        T Key { get; }

        G First { get; }

        G Last { get; }

        IMMethods<G> Select { get; }

        IMMethods<G> Where(Expression<Func<G, bool>> predicate);
    }

    public interface IMSearchQuery<T, Dto> : IMMapper<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMMapper<T, Dto> Where(Expression<Func<T, bool>> predicate);
    }

    public interface IMMapper<T, Dto> : IMGroupBy<Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMOrderBy<Dto> Map(Action<BHMapperConfig<T, Dto>> mapping);
    }

    public interface IMEnumerable<J, T> where T : class
    {
        IMOrderBy<T> Map(Func<J, T> selector);
    }

    public interface IMOrderBy<T> : IMQueryBase<T> where T : class
    {
        IMOrderByAfter<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderByAfter<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;
    }

    public interface IMOrderByAfter<T> : IMQueryBase<T> where T : class
    {
        IMOrderByAfter<T> ThenByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderByAfter<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMQueryBaseList<T> Take(int fetchRows);

        IMQueryBaseList<T> Skip(int offset);

        IMQueryBaseList<T> TakeWithOffset(int offsetRows, int fetchRows);
    }

    public interface IMQueryBase<T> : IMQueryBaseList<T> where T : class
    {
        T? FirstOrDefault(IBHTransaction? transaction = null);

        Task<T?> FirstOrDefaultAsync(IBHTransaction? transaction = null);

        T? LastOrDefeult(IBHTransaction? transaction = null);

        Task<T?> LastOrDefeultAsync(IBHTransaction? transaction = null);
    }

    public interface IMQueryBaseList<T> where T : class
    {
        List<T> ToList(IBHTransaction? transaction = null);

        Task<List<T>> ToListAsync(IBHTransaction? transaction = null);
    }

    #endregion

    #region Update Insert And Delete
    public interface IMUpdateQuery<T> where T : BHEntity<T>, new()
    {
        IMUpdateMapper<T> Where(Expression<Func<T, bool>> predicate);
    }

    public interface IMUpdateQuery<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMUpdateMapper<T, Dto> Where(Expression<Func<T, bool>> predicate);
    }

    public interface IMDeleteQuery<T> where T : BHEntity<T>, new()
    {
        IMExecute<T> Where(Expression<Func<T, bool>> predicate);

        IMExecute<T> All();
    }

    public interface IMUpdateMapper<T, Dto> : IMExecute<Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMExecute<Dto> UseColumns(Action<BHMappingConfig<T, Dto>> mapping);
    }

    public interface IMUpdateMapper<T> : IMExecute<T> where T : BHEntity<T>, new()
    {
        IMExecute<T> UseColumns(Action<BHColumnUsageConfig<T>> mapping);
    }

    public interface IMExecute<T> where T : class
    {
        Task<bool> ExecuteAsync(IBHTransaction? transaction = null);

        bool Execute(IBHTransaction? transaction = null);
    }
    #endregion

    #region Select AI
    public interface IMQueryAI<T, G> : IMIncludeAI<T, G>
       where T : BHEntityAI<T, G>, new() where G : struct, IBHStruct
    {
        IMSelectAI<T> InclideEverything();
    }

    public interface IMIncludeAI<T, L, Dto> : IMMapperAI<T, L, Dto>
        where T : BHEntityAI<T, L>, new() where Dto : BHDto<L> where L : struct, IBHStruct
    {
        IMThenIncludeAI<T, L, G, Dto> Include<G, H>(Func<T, BHCollection<G, H>> predicate)
            where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;

        IMThenIncludeAI<T, L, G, Dto> Include<G, H>(Func<T, BHItem<G, H>> predicate)
            where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;
    }

    public interface IMThenIncludeAI<T, L, G, Dto> : IMIncludeAI<T, L, Dto> where T : BHEntityAI<T, L>, new()
        where G : BHEntity<G>, new() where Dto : BHDto<L> where L : struct, IBHStruct
    {
        IMThenIncludeAI<T, L, J, Dto> ThenInclude<J, H>(Func<G, BHCollection<J, H>> predicate)
            where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;

        IMThenIncludeAI<T, L, J, Dto> ThenInclude<J, H>(Func<G, BHItem<J, H>> predicate)
            where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;
    }

    public interface IMIncludeAI<T, L> : IMSelectAI<T>
        where T : BHEntityAI<T, L>, new() where L : struct, IBHStruct
    {
        IMThenIncludeAI<T, L, G> Include<G, H>(Func<T, BHCollection<G, H>> predicate)
            where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;

        IMThenIncludeAI<T, L, G> Include<G, H>(Func<T, BHItem<G, H>> predicate)
            where G : BHEntityAI<G, H>, new() where H : struct, IBHStruct;
    }

    public interface IMThenIncludeAI<T, L, G> : IMIncludeAI<T, L>
        where T : BHEntityAI<T, L>, new() where G : BHEntity<G>, new() where L : struct, IBHStruct
    {
        IMThenIncludeAI<T, L, J> ThenInclude<J, H>(Func<G, BHCollection<J, H>> predicate)
            where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;

        IMThenIncludeAI<T, L, J> ThenInclude<J, H>(Func<G, BHItem<J, H>> predicate)
            where J : BHEntityAI<J, H>, new() where H : struct, IBHStruct;
    }

    public interface IMMapperAI<T, L, Dto> : IMSelectAI<Dto>
        where T : BHEntityAI<T, L>, new() where Dto : BHDto<L> where L : struct, IBHStruct
    {
        IMSelectAI<Dto> UseColumns(Action<BHMappingConfig<T, Dto>> mapping);
    }

    public interface IMSelectAI<T>
        where T : class
    {
        T? FirstOrDefault(IBHTransaction? transaction = null);

        Task<T?> FirstOrDefaultAsync(IBHTransaction? transaction = null);
    }

    #endregion

    #region Joins
    public interface IMJoinsProcess<Dto> : IMGroupBy<Dto> where Dto : class
    {
        IPreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, TSource, TOther> RightJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();
    }

    public interface IPreJoin<Dto, TSource, TOther> where Dto : class where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new()
    {
        IJoinConfig<Dto, TSource, TOther> On<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey) where TKey : IComparable;
    }

    public interface IJoinConfig<Dto, TSource, TOther> : IMJoinsProcess<Dto> where Dto : class where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new()
    {
        IJoinConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

        IJoinConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

        IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);
    }

    public interface IJoinOptions<Dto, TSource, TOther> : IMJoinsProcess<Dto> where Dto : class
    {
        IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);
    }
    #endregion
}