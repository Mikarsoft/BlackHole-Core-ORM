using Mikarsoft.BlackHoleCore.Connector;
using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Entities;
using Mikarsoft.BlackHoleCore.Tools;
using System.Linq.Expressions;
using System.Reflection;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHQueryUpdatable<T> : IBHQueryUpdatable<T> where T : BHEntity<T>
    {
        internal readonly IBHDataProvider _dataProvider;

        internal readonly List<T> Values;

        public BHQueryUpdatable(T value, IBHDataProvider dataProvider)
        {
            Values = new List<T>
            {
                value
            };

            _dataProvider = dataProvider;
        }

        public BHQueryUpdatable(List<T> values, IBHDataProvider dataProvider)
        {
            Values = values;
            _dataProvider = dataProvider;
        }

        bool IBHQueryUpdatable<T>.OnColumns(Action<UpdateSelection<T>> selection)
        {
            UpdateSelection<T> model = new();
            selection.Invoke(model);
            throw new NotImplementedException();
        }

        Task<bool> IBHQueryUpdatable<T>.OnColumnsAsync(Action<UpdateSelection<T>> selection)
        {
            UpdateSelection<T> model = new();
            selection.Invoke(model);

            IBHDataProvider provider = BHServiceInjector.GetDataProvider();
            throw new NotImplementedException();
        }

        bool IBHQueryUpdatable<T>.AllColumns()
        {
            throw new NotImplementedException();
        }

        Task<bool> IBHQueryUpdatable<T>.AllColumnsAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHQueryUpdatable<T, Dto> : IBHQueryUpdatable<T, Dto> where Dto : class where T : BHEntity<T>
    {
        private readonly IBHDataProvider _dataProvider;

        internal BHQueryUpdatable(IBHDataProvider bHDataProvider)
        {
            _dataProvider = bHDataProvider;
        }

        bool IBHQueryUpdatable<T, Dto>.AllMatchingColumns()
        {
            throw new NotImplementedException();
        }

        Task<bool> IBHQueryUpdatable<T, Dto>.AllMatchingColumnsAsync()
        {
            throw new NotImplementedException();
        }

        bool IBHQueryUpdatable<T, Dto>.MapColumns(Action<UpdateSelection<T, Dto>> selection)
        {
            UpdateSelection<T, Dto> model = new();
            selection.Invoke(model);
            throw new NotImplementedException();
        }

        Task<bool> IBHQueryUpdatable<T, Dto>.MapColumnsAsync(Action<UpdateSelection<T, Dto>> selection)
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
    internal class BHQueryJoinable<Dto, T> : BHQuery<T>, IBHQueryJoinable<Dto, T> where Dto : class where T : BHEntity<T>
    {

        IBHIncludeJoinable<T, G, Dto> IBHQueryJoinable<Dto, T>.Include<G>(Expression<Func<T, BHCollection<G>>> predicate)
        {
            throw new NotImplementedException();
        }

        public IBHQueryBase<Dto> Where(Expression<Func<T, bool>> predicate)
        {
            StatementBuilder.AddWhereCase(predicate);
            return new BHQuery<Dto>(StatementBuilder);
        }

        IPreJoin<Dto, T, TOther> IBHQueryJoinable<Dto, T>.InnerJoin<TOther>()
        {
            byte[] tableLetters = StatementBuilder.AddJoin<T, TOther>(JoinType.Inner);
            return new PreJoin<Dto, T, TOther>(StatementBuilder, tableLetters);
        }

        IPreJoin<Dto, T, TOther> IBHQueryJoinable<Dto, T>.LeftJoin<TOther>()
        {
            byte[] tableLetters = StatementBuilder.AddJoin<T, TOther>(JoinType.Left);
            return new PreJoin<Dto, T, TOther>(StatementBuilder, tableLetters);
        }

        IPreJoin<Dto, T, TOther> IBHQueryJoinable<Dto, T>.OuterJoin<TOther>()
        {
            byte[] tableLetters = StatementBuilder.AddJoin<T, TOther>(JoinType.Outer);
            return new PreJoin<Dto, T, TOther>(StatementBuilder, tableLetters);
        }

        IPreJoin<Dto, T, TOther> IBHQueryJoinable<Dto, T>.RightJoin<TOther>()
        {
            byte[] tableLetters = StatementBuilder.AddJoin<T, TOther>(JoinType.Right);
            return new PreJoin<Dto, T, TOther>(StatementBuilder, tableLetters);
        }
    }

    internal class BHQuerySearchable<T> : BHQuery<T>, IBHQuerySearchable<T> where T : BHEntity<T>
    {
        IBHInclude<T, G> IBHQuerySearchable<T>.Include<G>(Expression<Func<T, BHCollection<G>>> predicate)
        {
            throw new NotImplementedException();
        }

        IBHQueryBase<T> IBHQuerySearchable<T>.Where(Expression<Func<T, bool>> predicate)
        {
            StatementBuilder.AddWhereCase(predicate);
            return new BHQuery<T>(StatementBuilder);
        }
    }

    internal class BHInclude<T, G> : IBHInclude<T, G> where T : class where G : BHEntity<G>
    {
        IBHThenInclude<T, G> IBHInclude<T, G>.Match<TKey>(Expression<Func<T, TKey>> key, Expression<Func<G, TKey>> otherKey)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHInclude<T, G, D> : IBHInclude<T, G, D> where T : class where G : BHEntity<G> where D : BHEntity<D>
    {
        IBHThenInclude<T, D> IBHInclude<T, G, D>.Match<TKey>(Expression<Func<G, TKey>> key, Expression<Func<D, TKey>> otherKey)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHThenInclude<T, G> : BHQuery<T>, IBHThenInclude<T, G> where G : BHEntity<G> where T : class
    {
        IBHInclude<T, D> IBHThenInclude<T, G>.Include<D>(Expression<Func<T, BHCollection<D>>> predicate)
        {
            throw new NotImplementedException();
        }

        IBHInclude<T, G, D> IBHThenInclude<T, G>.ThenInclude<D>(Expression<Func<G, BHCollection<D>>> predicate)
        {
            throw new NotImplementedException();
        }

        IBHQueryBase<T> IBHThenInclude<T, G>.Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class BHQuery<T> : IBHQueryBase<T> where T : class
    {
        internal readonly BHSelectStatementBuilder StatementBuilder;

        internal BHQuery()
        {
            StatementBuilder = new(BHExpressionPartType.Select, typeof(T));
        }

        internal BHQuery(BHSelectStatementBuilder statement)
        {
            StatementBuilder = statement;
        }

        T? IBHQueryBase<T>.FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        Task<T?> IBHQueryBase<T>.FirstOrDefaultAsync()
        {
            throw new NotImplementedException();
        }

        IBHEnumerable<IBHGroup<G, T>, T> IBHQueryBase<T>.GroupBy<G>(Expression<Func<T, G>> keySelectors)
        {
            PropertyInfo[] groupProps = typeof(G).GetProperties();
            return new BHEnumerable<IBHGroup<G, T>, T>();
        }

        IBHOrderBy<T> IBHQueryBase<T>.OrderByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        IBHOrderBy<T> IBHQueryBase<T>.OrderByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        List<T> IBHQueryBase<T>.ToList()
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IBHQueryBase<T>.ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHEnumerable<T, TResult> : IBHEnumerable<T, TResult> where TResult : class
    {
        public IBHGroupedQuery<TResult> Map(Func<T, TResult> selector)
        {
            return new BHGroupedQuery<TResult>();
        }
    }

    internal class BHGroupedQuery<T> : IBHGroupedQuery<T> where T : class
    {
        IBHOrderByQuery<T> IBHGroupedQuery<T>.OrderByAscending(Expression<Func<T, object?>> action)
        {
            throw new NotImplementedException();
        }

        IBHOrderByQuery<T> IBHGroupedQuery<T>.OrderByDescending(Expression<Func<T, object?>> action)
        {
            throw new NotImplementedException();
        }

        List<T> IBHGroupedQuery<T>.ToList()
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IBHGroupedQuery<T>.ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHOrderByQuery<T> : IBHOrderByQuery<T> where T : class
    {
        IBHOrderByQuery<T> IBHOrderByQuery<T>.ThenByAscending<TKey>(Expression<Func<T, TKey?>> action) where TKey : default
        {
            throw new NotImplementedException();
        }

        IBHOrderByQuery<T> IBHOrderByQuery<T>.ThenByDescending<TKey>(Expression<Func<T, TKey?>> action) where TKey : default
        {
            throw new NotImplementedException();
        }

        IBHFinalQuery<T> IBHOrderByQuery<T>.Take(int fetchRows)
        {
            throw new NotImplementedException();
        }

        IBHFinalQuery<T> IBHOrderByQuery<T>.TakeWithOffset(int offsetRows, int fetchRows)
        {
            throw new NotImplementedException();
        }

        List<T> IBHOrderByQuery<T>.ToList()
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IBHOrderByQuery<T>.ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHFinalQuery<T> : IBHFinalQuery<T> where T : class
    {
        List<T> IBHFinalQuery<T>.ToList()
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IBHFinalQuery<T>.ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class BHGroup<T, G> : IBHGroup<T, G>
    {
        private readonly T _key;

        internal BHGroup(T key)
        {
            _key = key;
        }

        public T Key => _key;

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
