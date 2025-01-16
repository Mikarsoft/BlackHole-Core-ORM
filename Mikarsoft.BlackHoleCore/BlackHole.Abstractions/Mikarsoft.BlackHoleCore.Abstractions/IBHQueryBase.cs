using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IBHQueryJoinable<Dto, T> : IBHQueryJoinableBase<Dto, T> where Dto : class where T : BHEntity<T> , new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, T, TOther> InnerJoin<TOther>() where TOther : BHEntity<TOther> , new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, T, TOther> OuterJoin<TOther>() where TOther : BHEntity<TOther>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, T, TOther> LeftJoin<TOther>() where TOther : BHEntity<TOther> , new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        IPreJoin<Dto, T, TOther> RightJoin<TOther>() where TOther : BHEntity<TOther> , new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHQueryBase<Dto> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="G"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHIncludeJoinable<T, G, Dto> Include<G>(Expression<Func<T, BHCollection<G>>> key) where G : BHEntity<G> , new();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHQueryUpdatable<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool AllColumns();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> AllColumnsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool OnColumns(Action<UpdateSelection<T>> selection);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> OnColumnsAsync(Action<UpdateSelection<T>> selection);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Dto"></typeparam>
    public interface IBHQueryUpdatable<T, Dto> where Dto : class where T : BHEntity<T>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool AllMatchingColumns();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> AllMatchingColumnsAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool MapColumns(Expression<Func<Dto, T>> selector);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> MapColumnsAsync(Expression<Func<Dto, T>> selector);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Dto"></typeparam>
    public class UpdateSelection<T, Dto> : UpdateSelection<T> where T : BHEntity<T>, new()
    {
        private readonly Dictionary<string, string> PropertyMappings = new();

        public Dictionary<string, string> Mapping => PropertyMappings;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dtoKey"></param>
        /// <param name="entityKey"></param>
        public void Map<TKey>(Expression<Func<Dto, TKey?>> dtoKey, Expression<Func<T, TKey?>> entityKey) where TKey : IComparable
        {
            string dto = dtoKey.MemberParse();
            string entity = entityKey.MemberParse();
            PropertyMappings.Add(dto, entity);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateSelection<T> where T : class
    {
        private readonly List<string> PropertyNames = new();
        public List<string> UsedProperties => PropertyNames;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        public void Use<TKey>(Expression<Func<T, TKey?>> key) where TKey : IComparable
        {
            string column = key.MemberParse();
            PropertyNames.Add(column);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHQuerySearchable<T> : IBHQueryBase<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHQueryBase<T> Where(Expression<Func<T, bool>> predicate);

        IBHInclude<T, G> Include<G>(Expression<Func<T, BHCollection<G>>> predicate) where G : BHEntity<G>, new();
    }

    public interface IBHIncludeJoinable<T, G, Dto> where T : BHEntity<T> , new() where Dto : class where G : BHEntity<G>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IBHThenIncludeJoinable<T, G, Dto> Match<TKey>(Expression<Func<T, TKey>> parentKey, Expression<Func<G, TKey>> childKey) where TKey : IComparable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHIncludeJoinable<T, G, D, Dto> where T : BHEntity<T> , new() where Dto : class where G : BHEntity<G>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IBHIncludeJoinable<T, G, Dto> Match<TKey>(Expression<Func<T, TKey>> key, Expression<Func<G, TKey>> otherKey) where TKey : IComparable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHThenIncludeJoinable<T, G, Dto> : IBHQueryJoinableBase<Dto, T> where T : BHEntity<T>, new() where Dto : class where G : BHEntity<G>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHQueryJoinableBase<Dto, T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHIncludeJoinable<T, D, Dto> Include<D>(Expression<Func<T, BHCollection<D>>> predicate) where D : BHEntity<D>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHIncludeJoinable<T, G, D, Dto> ThenInclude<D>(Expression<Func<G, BHCollection<D>>> predicate) where D : BHEntity<D>, new();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHInclude<T, G> where T : class where G : BHEntity<G>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IBHThenInclude<T, G> Match<TKey>(Expression<Func<T, TKey>> key, Expression<Func<G, TKey>> otherKey) where TKey : IComparable;
    }

    public interface IBHInclude<T, G, D> where T : class where G : BHEntity<G> , new() where D : BHEntity<D> ,new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherKey"></param>
        /// <returns></returns>
        IBHThenInclude<T, D> Match<TKey>(Expression<Func<G, TKey>> key, Expression<Func<D, TKey>> otherKey);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHThenInclude<T, G> : IBHQueryBase<T> where T : class where G : BHEntity<G>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHQueryBase<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHInclude<T, D> Include<D>(Expression<Func<T, BHCollection<D>>> predicate) where D : BHEntity<D>, new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IBHInclude<T, G, D> ThenInclude<D>(Expression<Func<G, BHCollection<D>>> predicate) where D : BHEntity<D>, new();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHQueryBase<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderBy<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderBy<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action) where TKey : IComparable;

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
    public interface IBHFinalQuery<T> where T : class
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
    public interface IBHOrderByQuery<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> ThenByAscending<TKey>(Expression<Func<T, TKey?>> action) where TKey : IComparable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderByQuery<T> ThenByDescending<TKey>(Expression<Func<T, TKey?>> action) where TKey : IComparable;

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
    public interface IBHGroupedQuery<T> where T : class
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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHOrderBy<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderBy<T> ThenByAscending<TKey>(Expression<Func<T, TKey>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHOrderBy<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchRows"></param>
        /// <returns></returns>
        List<T> Take(int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetRows"></param>
        /// <param name="fetchRows"></param>
        /// <returns></returns>
        List<T> TakeWithOffset(int offsetRows, int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchRows"></param>
        /// <returns></returns>
        Task<List<T>> TakeAsync(int fetchRows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetRows"></param>
        /// <param name="fetchRows"></param>
        /// <returns></returns>
        Task<List<T>> TakeWithOffsetAsync(int offsetRows, int fetchRows);
    }
}
