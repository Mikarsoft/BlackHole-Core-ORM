using System.Linq.Expressions;

namespace BlackHole.Abstractions.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHQuery<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHQuery<T> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHQuery<T> OrderByDescending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHGroup<T,G> GroupBy<G>(Expression<Func<T, G?>> action);

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

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TElement> elementSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

            var lookup = new Dictionary<TKey, List<TElement>>();

            foreach (var item in source)
            {
                var key = keySelector(item);
                var element = elementSelector(item);

                if (!lookup.TryGetValue(key, out var elements))
                {
                    elements = new List<TElement>();
                    lookup[key] = elements;
                }

                elements.Add(element);
            }

            foreach (var pair in lookup)
            {
                yield return new Grouping<TKey, TElement>(pair.Key, pair.Value);
            }
        }
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
        /// <param name="action"></param>
        /// <returns></returns>
        IBHQuery<T> OrderByAscending(Expression<Func<T, object?>> action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IBHQuery<T> OrderByDescending(Expression<Func<T, object?>> action);

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


    private class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public TKey Key { get; }
        private readonly IEnumerable<TElement> _elements;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
    }
}
