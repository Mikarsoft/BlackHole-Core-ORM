using Mikarsoft.BlackHoleCore.Abstractions;
using Mikarsoft.BlackHoleCore.Abstractions.Models;
using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Entities;
using Mikarsoft.BlackHoleCore.Tools;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    #region Using Entity

    internal class MQuery<T> : MIncludeBase<T>, IMQuery<T> where T : BHEntity<T>, new()
    {
        internal MQuery() : base (new(BHExpressionPartType.Select, typeof(T))) { }

        BHTableInfo IMQuery<T>.TableInfo()
        {
            throw new NotImplementedException();
        }

        Task<BHTableInfo> IMQuery<T>.TableInfoAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class MIncludeBase<T> : MSearchQuery<T>, IMIncludeBase<T> where T : BHEntity<T>, new()
    {
        internal MIncludeBase(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMIncludeMatch<T, G> IMIncludeBase<T>.Include<G>(Func<T, BHCollection<G>> predicate)
        {
            StatementBuilder.UseInclude(predicate);
            return new MIncludeMatch<T, G>(StatementBuilder);
        }

        IMIncludeMatch<T, G> IMIncludeBase<T>.Include<G>(Func<T, BHItem<G>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class MIncludeMatch<T, G> : IMIncludeMatch<T, G> where T : BHEntity<T>, new() where G : BHEntity<G>, new()
    {
        internal BHSelectStatementBuilder StatementBuilder;

        internal MIncludeMatch(BHSelectStatementBuilder statementBuilder) { StatementBuilder = statementBuilder; }

        IMIncludeBase<T> IMIncludeMatch<T, G>.Match<TKey>(Expression<Func<T, TKey>> parentKey, Expression<Func<G, TKey>> childKey)
        {

            return new MIncludeBase<T>(StatementBuilder);
        }
    }

    internal class MSearchQuery<T> : MGroupBy<T>, IMSearchQuery<T> where T : BHEntity<T>, new()
    {
        internal MSearchQuery(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMGroupBy<T> IMSearchQuery<T>.Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Using Dto

    internal class MQuery<T, Dto> : MIncludeBase<T, Dto>, IMQuery<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        internal MQuery() : base(new(BHExpressionPartType.Select, typeof(Dto))) { }

        IPreJoin<Dto, T, TOther> IMQuery<T, Dto>.InnerJoin<TOther>()
        {
            throw new NotImplementedException();
        }

        IPreJoin<Dto, T, TOther> IMQuery<T, Dto>.LeftJoin<TOther>()
        {
            throw new NotImplementedException();
        }

        IPreJoin<Dto, T, TOther> IMQuery<T, Dto>.OuterJoin<TOther>()
        {
            throw new NotImplementedException();
        }

        IPreJoin<Dto, T, TOther> IMQuery<T, Dto>.RightJoin<TOther>()
        {
            throw new NotImplementedException();
        }
    }

    internal class MIncludeBase<T, Dto> : MSearchQuery<T, Dto>, IMIncludeBase<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        internal MIncludeBase(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMIncludeMatch<T, Dto, G> IMIncludeBase<T, Dto>.Include<G>(Func<Dto, BHCollection<G>> predicate)
        {
            throw new NotImplementedException();
        }

        IMIncludeMatch<T, Dto, G> IMIncludeBase<T, Dto>.Include<G>(Func<Dto, BHItem<G>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class MSearchQuery<T, Dto> : MGroupBy<Dto>, IMSearchQuery<T, Dto> where T : BHEntity<T>, new() where Dto : class
    {
        internal MSearchQuery(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMGroupBy<Dto> IMSearchQuery<T, Dto>.Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Common


    internal class MGroupBy<T> : MOrderBy<T>, IMGroupBy<T> where T : class
    {
        internal MGroupBy(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMEnumerable<IBHGroup<G, T>, T> IMGroupBy<T>.GroupBy<G>(Expression<Func<T, G>> keySelectors)
        {
            throw new NotImplementedException();
        }
    }

    internal class MOrderBy<T> : MQueryBase<T>, IMOrderBy<T> where T : class
    {
        internal MOrderBy(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMOrderByAfter<T> IMOrderBy<T>.OrderByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        IMOrderByAfter<T> IMOrderBy<T>.OrderByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }
    }

    internal class MOrderByAfter<T> : MQueryBase<T>, IMOrderByAfter<T> where T : class
    {
        internal MOrderByAfter(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        IMQueryBaseList<T> IMOrderByAfter<T>.Skip(int offset)
        {
            throw new NotImplementedException();
        }

        IMQueryBaseList<T> IMOrderByAfter<T>.Take(int fetchRows)
        {
            throw new NotImplementedException();
        }

        IMQueryBaseList<T> IMOrderByAfter<T>.TakeWithOffset(int offsetRows, int fetchRows)
        {
            throw new NotImplementedException();
        }

        IMOrderByAfter<T> IMOrderByAfter<T>.ThenByAscending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }

        IMOrderByAfter<T> IMOrderByAfter<T>.ThenByDescending<TKey>(Expression<Func<T, TKey>> action)
        {
            throw new NotImplementedException();
        }
    }

    internal class MQueryBase<T> : MQueryBaseList<T>, IMQueryBase<T> where T : class
    {
        internal MQueryBase(BHSelectStatementBuilder statementBuilder) : base(statementBuilder) { }

        T? IMQueryBase<T>.FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        Task<T?> IMQueryBase<T>.FirstOrDefaultAsync()
        {
            throw new NotImplementedException();
        }

        T? IMQueryBase<T>.LastOrDefeult()
        {
            throw new NotImplementedException();
        }

        Task<T?> IMQueryBase<T>.LastOrDefeultAsync()
        {
            throw new NotImplementedException();
        }
    }

    internal class MQueryBaseList<T> : IMQueryBaseList<T> where T : class
    {
        internal BHSelectStatementBuilder StatementBuilder;

        internal MQueryBaseList(BHSelectStatementBuilder statementBuilder)
        {
            StatementBuilder = statementBuilder;
        }

        List<T> IMQueryBaseList<T>.ToList()
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IMQueryBaseList<T>.ToListAsync()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
