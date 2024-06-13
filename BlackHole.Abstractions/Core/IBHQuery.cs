using BlackHole.Core;
using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Abstractions.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IBHQueryJoinable<Dto, T> : IBHQuerySearchable<Dto> where Dto : BHDto where T : BHEntityIdentifier
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, T, TOther> InnerJoin<TOther>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, T, TOther> OuterJoin<TOther>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, T, TOther> LeftJoin<TOther>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPrejoin<Dto, T, TOther> RightJoin<TOther>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHQuerySearchable<T> : IBHQuery<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHQuery<T> Where(Expression<Func<T, bool>> predicate);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHQuery<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        BHOrderBy<T> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        BHOrderBy<T> OrderByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <param name="keySelectors"></param>
        /// <returns></returns>
        IBHEnumerable<IBHGroup<G, T>, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T? FirstOrDefault();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<T?> FirstOrDefaultAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHFinalQuery<T> where T: class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHOrderByQuery<T> where T: class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> ThenByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> ThenByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchRows"></param>
        IBHFinalQuery<T> Take(int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetRows"></param>
        /// <param name="fetchRows"></param>
        IBHFinalQuery<T> TakeWithOffset(int offsetRows, int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHGroupedQuery<T> where T: class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> OrderByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHGroup<T, G>
    {
        /// <summary>
        /// 
        /// </summary>
        T Key { get; }

        /// <summary>
        /// 
        /// </summary>
        G First { get; }

        /// <summary>
        /// 
        /// </summary>
        G Last { get; }

        /// <summary>
        /// 
        /// </summary>
        IBHMethods<G> Select { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHMethods<G> Where(Expression<Func<G, bool>> predicate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="G"></typeparam>
    public interface IBHMethods<G>
    {
        /// <summary>
        /// 
        /// </summary>
        G First { get; }

        /// <summary>
        /// 
        /// </summary>
        G Last { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        int Max(Func<G, int?> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        double Max(Func<G, double?> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        decimal Max(Func<G, decimal?> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        long Max(Func<G, long?> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        short Max(Func<G, short?> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        DateTime Max(Func<G, DateTime?> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        DateTimeOffset Max(Func<G, DateTimeOffset?> selector);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IBHEnumerable<T, TResult> where TResult : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        IBHGroupedQuery<TResult> Map(Func<T, TResult> selector);
    }

    //public class BHGroup<TKey, TElement> where TElement : class
    //{
    //    public TKey Key { get; }
    //    public IEnumerable<TElement> Elements { get; }
    //    public TElement First { get; }

    //    public TElement Last { get; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="key"></param>
    //    /// <param name="elements"></param>
    //    public BHGroup(TKey key, IEnumerable<TElement> elements)
    //    {
    //        Key = key;
    //        Elements = elements;
    //        First = elements.First();
    //        Last = elements.Last();
    //    }

    //    public void Case(TKey key, IEnumerable<TElement> elements)
    //    {

    //    }
    //}

    //public class BHEnumerable<T,TResult>
    //{
    //    private readonly List<T> _items;
    //    public BHEnumerable()
    //    {
    //        _items = new List<T>();
    //    }

    //    public IBHQuery<TResult> Map(Func<T, TResult> selector)
    //    {
    //        if (source == null) throw new ArgumentNullException(nameof(source));
    //        if (selector == null) throw new ArgumentNullException(nameof(selector));

    //        return CustomSelectIterator(selector);
    //    }

    //    private IBHQuery<TResult> CustomSelectIterator(Func<T, TResult> selector)
    //    {
    //        foreach (var item in _items)
    //        {
    //            yield return selector(item);
    //        }
    //    }

    //    // Method to add items to the custom collection
    //    public void Add(T item)
    //    {
    //        _items.Add(item);
    //    }

    //    // Method to get enumerator
    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return _items.GetEnumerator();
    //    }
    //}
}
