using Mikarsoft.BlackHoleCore.Connector;
using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Connector.Statements;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHQueryUpdatable<T> : IBHQueryUpdatable<T> where T : BHEntity<T>
    {
        internal List<T> Values { get; private set; }

        public BHQueryUpdatable(T value)
        {
            Values = new List<T>
            {
                value
            };
        }

        public BHQueryUpdatable(List<T> values)
        {
            Values = values;
        }

        public bool OnColumns(Action<UpdateSelection<T>> selection)
        {
            UpdateSelection<T> model = new();
            selection.Invoke(model);
            throw new NotImplementedException();
        }

        public Task<bool> OnColumnsAsync(Action<UpdateSelection<T>> selection)
        {
            UpdateSelection<T> model = new();
            selection.Invoke(model);
            throw new NotImplementedException();
        }

        public bool AllColumns()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AllColumnsAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHQueryUpdatable<T, Dto> : IBHQueryUpdatable<T, Dto> where Dto : BHDto where T : BHEntity<T>
    {
        private readonly IBHDataProvider _dataProvider;

        internal BHQueryUpdatable(IBHDataProvider bHDataProvider)
        {
            _dataProvider = bHDataProvider;
        }

        public bool AllMatchingColumns()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AllMatchingColumnsAsync()
        {
            throw new NotImplementedException();
        }

        public bool MapColumns(Action<UpdateSelection<T, Dto>> selection)
        {
            UpdateSelection<T, Dto> model = new();
            selection.Invoke(model);
            throw new NotImplementedException();
        }

        public Task<bool> MapColumnsAsync(Action<UpdateSelection<T, Dto>> selection)
        {
            UpdateSelection<T, Dto> model = new();
            selection.Invoke(model);
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="T"></typeparam>
    internal class BHQueryJoinable<Dto, T> : BHQuerySearchable<Dto>, IBHQueryJoinable<Dto, T> where Dto : BHDto where T : BHEntity<T>
    {
        public IPreJoin<Dto, T, TOther> InnerJoin<TOther>() where TOther : BHEntity<TOther>
        {
            return new PreJoin<Dto, T, TOther>(new JoinStatement<Dto>(nameof(T), nameof(TOther), JoinType.Inner));
        }

        public IPreJoin<Dto, T, TOther> LeftJoin<TOther>() where TOther : BHEntity<TOther>
        {
            return new PreJoin<Dto, T, TOther>(new JoinStatement<Dto>(nameof(T), nameof(TOther), JoinType.Left));
        }

        public IPreJoin<Dto, T, TOther> OuterJoin<TOther>() where TOther : BHEntity<TOther>
        {
            return new PreJoin<Dto, T, TOther>(new JoinStatement<Dto>(nameof(T), nameof(TOther), JoinType.Outer));
        }

        public IPreJoin<Dto, T, TOther> RightJoin<TOther>() where TOther : BHEntity<TOther>
        {
            return new PreJoin<Dto, T, TOther>(new JoinStatement<Dto>(nameof(T), nameof(TOther), JoinType.Right));
        }
    }

    internal class BHQuerySearchable<T> : BHQuery<T>, IBHQuerySearchable<T> where T : class
    {
        public IBHQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHQuery<T> : IBHQuery<T> where T : class
    {
        public T? FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public Task<T?> FirstOrDefaultAsync()
        {
            throw new NotImplementedException();
        }

        public IBHEnumerable<IBHGroup<G, T>, T> GroupBy<G>(Expression<Func<T, G>> keySelectors)
        {
            throw new NotImplementedException();
        }

        public IBHOrderBy<T> OrderByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        public IBHOrderBy<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        public List<T> ToList()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHEnumerable<T, TResult> : IBHEnumerable<T, TResult> where TResult : class
    {
        public IBHGroupedQuery<TResult> Map(Func<T, TResult> selector)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHGroupedQuery<T> : IBHGroupedQuery<T> where T : class
    {
        public IBHOrderByQuery<T> OrderByAscending(Expression<Func<T, object?>> action)
        {
            throw new NotImplementedException();
        }

        public IBHOrderByQuery<T> OrderByDescending(Expression<Func<T, object?>> action)
        {
            throw new NotImplementedException();
        }

        public List<T> ToList()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHOrderByQuery<T> : IBHOrderByQuery<T> where T : class
    {
        public IBHFinalQuery<T> Take(int fetchRows)
        {
            throw new NotImplementedException();
        }

        public IBHFinalQuery<T> TakeWithOffset(int offsetRows, int fetchRows)
        {
            throw new NotImplementedException();
        }

        public IBHOrderByQuery<T> ThenByAscending(Expression<Func<T, object?>> action)
        {
            throw new NotImplementedException();
        }

        public IBHOrderByQuery<T> ThenByDescending(Expression<Func<T, object?>> action)
        {
            throw new NotImplementedException();
        }

        public List<T> ToList()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHFinalQuery<T> : IBHFinalQuery<T> where T : class
    {
        public List<T> ToList()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHGroup<T, G> : IBHGroup<T, G>
    {
        public T Key => throw new NotImplementedException();

        public G First => throw new NotImplementedException();

        public G Last => throw new NotImplementedException();

        public IBHMethods<G> Select => throw new NotImplementedException();

        public IBHMethods<G> Where(Expression<Func<G, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHMethods<G> : IBHMethods<G>
    {
        public G First => throw new NotImplementedException();

        public G Last => throw new NotImplementedException();

        public int Max(Func<G, int?> selector)
        {
            throw new NotImplementedException();
        }

        public double Max(Func<G, double?> selector)
        {
            throw new NotImplementedException();
        }

        public decimal Max(Func<G, decimal?> selector)
        {
            throw new NotImplementedException();
        }

        public long Max(Func<G, long?> selector)
        {
            throw new NotImplementedException();
        }

        public short Max(Func<G, short?> selector)
        {
            throw new NotImplementedException();
        }

        public DateTime Max(Func<G, DateTime?> selector)
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset Max(Func<G, DateTimeOffset?> selector)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHOrderBy<T> : IBHOrderBy<T> where T : class
    {
        public List<T> Take(int fetchRows)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> TakeAsync(int fetchRows)
        {
            throw new NotImplementedException();
        }

        public List<T> TakeWithOffset(int offsetRows, int fetchRows)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> TakeWithOffsetAsync(int offsetRows, int fetchRows)
        {
            throw new NotImplementedException();
        }

        public IBHOrderBy<T> ThenByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        public IBHOrderBy<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }
    }
}
