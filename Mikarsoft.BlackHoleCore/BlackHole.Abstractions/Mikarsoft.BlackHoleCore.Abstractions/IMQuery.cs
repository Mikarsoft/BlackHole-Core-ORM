

using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore.Abstractions
{
    public interface IMQuery<T> : IMQueryBase<T> where T : BHEntity<T>
    {
        IMOrderBy<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderBy<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IBHEnumerable<IBHGroup<G, T>, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);

        IMQuery<T> Where(Expression<Func<T, bool>> predicate);

        IBHInclude<T, G> Include<G>(Expression<Func<T, BHCollection<G>>> predicate) where G : BHEntity<G>;
    }

    public interface IMQuery<T, Dto> : IMIncludeBase<T, Dto> where T : BHEntity<T> where Dto : class
    {
        IPreJoin<Dto, T, TOther> InnerJoin<TOther>() where TOther : BHEntity<TOther>;

        IPreJoin<Dto, T, TOther> OuterJoin<TOther>() where TOther : BHEntity<TOther>;

        IPreJoin<Dto, T, TOther> LeftJoin<TOther>() where TOther : BHEntity<TOther>;

        IPreJoin<Dto, T, TOther> RightJoin<TOther>() where TOther : BHEntity<TOther>;
    }

    public interface IMIncludeBase<T, Dto> : IMSearchQuery<T, Dto> where T : BHEntity<T> where Dto : class
    {
        IMIncludeMatch<T, Dto, G> Include<G>(Expression<Func<T, BHCollection<G>>> predicate) where G : BHEntity<G>;
    }

    //public interface IMInclude<T, Dto, G> : IMIncludeBase<T, Dto> where T : BHEntity<T> where Dto : class where G : BHEntity<G>
    //{
    //    IMIncludeAfter<T, G, D, Dto> ThenInclude<D>(Expression<Func<G, BHCollection<D>>> predicate) where D : BHEntity<D>;
    //}

    //public interface IMIncludeAfter<T, G, D, Dto> where T : BHEntity<T> where Dto : class where G : BHEntity<G>
    //{
    //    IMInclude<T, Dto, G> Match<TKey>(Expression<Func<T, TKey>> key, Expression<Func<G, TKey>> otherKey) where TKey : IComparable;
    //}

    public interface IMIncludeMatch<T, Dto, G> where T : BHEntity<T> where Dto : class where G : BHEntity<G>
    {
        IMIncludeBase<T, Dto> Match<TKey>(Expression<Func<T, TKey>> parentKey, Expression<Func<G, TKey>> childKey) where TKey : IComparable;
    }

    public interface IMSearchQuery<T, Dto> : IMGroupBy<T, Dto> where T : BHEntity<T> where Dto : class
    {
        IMGroupBy<T, Dto> Where(Expression<Func<T, bool>> predicate);
    }

    public interface IMGroupBy<T, Dto> : IMOrderBy<T, Dto> where T : BHEntity<T> where Dto : class
    {
        IMOrderBy<T, Dto> Map(Expression<Func<T, Dto>> selector);
        IMEnumerable<IBHGroup<G, T>, Dto, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);
    }

    public interface IMEnumerable<J, Dto, T> where T : BHEntity<T> where Dto : class
    {
        IMOrderBy<T, Dto> Map(Func<J, Dto> selector);
    }

    public interface IMEnumerable<J, T> where T : BHEntity<T>
    {
        IMOrderBy<T> Map(Func<J, T> selector);
    }

    public interface IMOrderBy<T> : IMQueryBase<T> where T : BHEntity<T>
    {
        IMOrderByAfter<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderByAfter<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;
    }

    public interface IMOrderBy<T, Dto> : IMQueryBase<Dto> where T : BHEntity<T> where Dto: class
    {
        IMOrderByAfter<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderByAfter<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;
    }

    public interface IMOrderByAfter<T> : IMQueryBase<T> where T : BHEntity<T>
    {
        IMOrderByAfter<T> ThenByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderByAfter<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMQueryBaseList<T> Take(int fetchRows);

        IMQueryBaseList<T> Skip(int offset);

        IMQueryBaseList<T> TakeWithOffset(int offsetRows, int fetchRows);
    }

    public interface IMOrderByAfter<T, Dto> : IMQueryBase<Dto> where T : BHEntity<T> where Dto : class
    {
        IMOrderByAfter<T, Dto> ThenByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMOrderByAfter<T, Dto> ThenByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        IMQueryBaseList<Dto> Take(int fetchRows);

        IMQueryBaseList<T> Skip(int offset);

        IMQueryBaseList<Dto> TakeWithOffset(int offsetRows, int fetchRows);
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
