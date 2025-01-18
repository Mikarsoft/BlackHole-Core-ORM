

using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    public interface IMQuery<T> : IMIncludeBase<T> where T : BHEntity<T>, new()
    {
        Task<BHTableInfo> TableInfoAsync();
        BHTableInfo TableInfo();
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
        IMThenInclude<T, G> Include<G>(Func<T, BHCollection<G>> predicate) where G : BHEntity<G>, new();
        IMThenInclude<T, G> Include<G>(Func<T, BHItem<G>> predicate) where G : BHEntity<G>, new();
    }

    public interface IMIncludeBase<T, Dto> : IMSearchQuery<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        IMThenInclude<T, G, Dto> Include<G>(Func<Dto, BHCollection<G>> predicate) where G : BHEntity<G>, new();
        IMThenInclude<T, G, Dto> Include<G>(Func<Dto, BHItem<G>> predicate) where G : BHEntity<G>, new();
    }

    public interface IMThenInclude<T, G> : IMIncludeBase<T> where T :BHEntity<T>, new() where G : BHEntity<G>, new()
    {
        IMThenInclude<T, J> ThenInclude<J>(Func<T, BHCollection<J>> predicate) where J : BHEntity<J>, new();
        IMThenInclude<T, J> ThenInclude<J>(Func<T, BHItem<J>> predicate) where J : BHEntity<J>, new();
    }

    public interface IMThenInclude<T, G, Dto> : IMIncludeBase<T, Dto> where T : BHEntity<T>, new() where G : BHEntity<G>, new() where Dto : class
    {
        IMThenInclude<T, J, Dto> ThenInclude<J>(Func<T, BHCollection<J>> predicate) where J : BHEntity<J>, new();
        IMThenInclude<T, J, Dto> ThenInclude<J>(Func<T, BHItem<J>> predicate) where J : BHEntity<J>, new();
    }

    public interface IMSearchQuery<T> : IMGroupBy<T> where T : BHEntity<T>, new()
    {
        IMGroupBy<T> Where(Expression<Func<T, bool>> predicate);
    }

    public interface IMGroupBy<T> : IMOrderBy<T> where T : class
    {
        IMEnumerable<IBHGroup<G, T>, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);
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
        T? FirstOrDefault();

        Task<T?> FirstOrDefaultAsync();

        T? LastOrDefeult();

        Task<T?> LastOrDefeultAsync();
    }

    public interface IMQueryBaseList<T> where T : class
    {
        List<T> ToList();

        Task<List<T>> ToListAsync();
    }
}