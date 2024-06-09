using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Collections;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace BlackHole.Abstractions.Core
{
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
        /// <typeparam name="G"></typeparam>
        /// <param name="keySelectors"></param>
        /// <returns></returns>
        CustomEnumerable<CustomGroup<G, T>, T> GroupBy<G>(Expression<Func<T, G>> keySelectors);

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
        //public static IEnumerable<string> GetGroupByFieldNames<TSource>(params Expression<Func<TSource, object>>[] keySelectors)
        //{
        //    foreach (var keySelector in keySelectors)
        //    {
        //        var memberExpression = GetMemberExpression(keySelector.Body);
        //        if (memberExpression != null)
        //        {
        //            yield return memberExpression.Member.Name;
        //        }
        //    }
        //}

        //private static MemberExpression GetMemberExpression(Expression expression)
        //{
        //    if (expression is MemberExpression memberExpression)
        //    {
        //        return memberExpression;
        //    }

        //    if (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
        //    {
        //        return unaryExpression.Operand as MemberExpression;
        //    }

        //    return null;
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHGroup<T, G>
    {
        IBHQuery<T> Select();
    }


    public static class CustomLinqExtensions
    {
        // Custom Grouping class
        private class BHGrouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            public TKey Key { get; }

            private readonly IEnumerable<TElement> _elements;

            public BHGrouping(TKey key, IEnumerable<TElement> elements)
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

        // Custom GroupBy method
        //public static IEnumerable<IGrouping<TKey, TElement>> CustomGroupBy<TSource, TKey, TElement>(
        //    this CustomEnumerable<TSource> source,
        //    Func<TSource, TKey> keySelector,
        //    Func<TSource, TElement> elementSelector)
        //{
        //    if (source == null) throw new ArgumentNullException(nameof(source));
        //    if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        //    if (elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

        //    return CustomGroupByIterator(source, keySelector, elementSelector);
        //}

        private static IEnumerable<IGrouping<TKey, TElement>> CustomGroupByIterator<TSource, TKey, TElement>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
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
                yield return new BHGrouping<TKey, TElement>(pair.Key, pair.Value);
            }
        }


        public static IEnumerable<TResult> CustomSelect<TSource, TResult>(
        this CustomEnumerable<TSource> source,
        Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return CustomSelectIterator(source, selector);
        }


        // Custom Select method (not necessary to customize, just use the standard one)
    }


    public class CustomGroup<TKey, TElement> where TElement : class
    {
        public TKey Key { get; }
        public IEnumerable<TElement> Elements { get; }
        public TElement First { get; }

        public TElement Last { get; }
        public CustomGroup(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            Elements = elements;
            First = elements.First();
            Last = elements.Last();
        }

        public void Case(TKey key, IEnumerable<TElement> elements)
        {

        }
    }

    public class CustomEnumerable<T,TResult>
    {
        private readonly List<T> _items;
        public CustomEnumerable()
        {
            _items = new List<T>();
        }

        public IEnumerable<TResult> Map(Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return CustomSelectIterator(selector);
        }

        private IEnumerable<TResult> CustomSelectIterator(Func<T, TResult> selector)
        {
            foreach (var item in _items)
            {
                yield return selector(item);
            }
        }

        // Method to add items to the custom collection
        public void Add(T item)
        {
            _items.Add(item);
        }

        // Method to get enumerator
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
