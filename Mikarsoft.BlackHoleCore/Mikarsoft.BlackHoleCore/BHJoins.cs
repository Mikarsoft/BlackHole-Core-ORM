using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Entities;
using Mikarsoft.BlackHoleCore.Settings;
using Mikarsoft.BlackHoleCore.Tools;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal class MJoinsProcess<Dto> : MGroupBy<Dto> , IMJoinsProcess<Dto> 
        where Dto : class
    {
        internal MJoinsProcess(BHSelectStatementBuilder statement) : base(statement) { }

        IPreJoin<Dto, TSource, TOther> IMJoinsProcess<Dto>.InnerJoin<TSource, TOther>()
        {
            throw new NotImplementedException();
        }

        IPreJoin<Dto, TSource, TOther> IMJoinsProcess<Dto>.LeftJoin<TSource, TOther>()
        {
            throw new NotImplementedException();
        }

        IPreJoin<Dto, TSource, TOther> IMJoinsProcess<Dto>.OuterJoin<TSource, TOther>()
        {
            throw new NotImplementedException();
        }

        IPreJoin<Dto, TSource, TOther> IMJoinsProcess<Dto>.RightJoin<TSource, TOther>()
        {
            throw new NotImplementedException();
        }
    }

    internal class PreJoin<Dto, TSource, TOther> : IPreJoin<Dto, TSource, TOther> 
        where Dto : class 
        where TSource : BHEntity<TSource> , new()
        where TOther : BHEntity<TOther> , new()
    {
        private readonly BHSelectStatementBuilder StatementBuilder;
        private readonly byte TableACode;
        private readonly byte TableDCode;

        internal PreJoin(BHSelectStatementBuilder statement, byte[] tableLetters)
        {
            StatementBuilder = statement;
            TableACode = tableLetters[0];
            TableDCode = tableLetters[1];
        }

        IJoinConfig<Dto, TSource, TOther> IPreJoin<Dto, TSource, TOther>.On<TKey>(Expression<Func<TSource, TKey?>> key, 
            Expression<Func<TOther, TKey?>> otherKey) where TKey : default
        {
            throw new NotImplementedException();
        }
    }

    internal class JoinConfig<Dto, TSource, TOther> : MGroupBy<Dto>, IJoinConfig<Dto, TSource, TOther> 
        where Dto : class 
        where TSource : BHEntity<TSource> , new()
        where TOther : BHEntity<TOther> , new()
    {
        private readonly byte TableACode;
        private readonly byte TableDCode;

        internal JoinConfig(BHSelectStatementBuilder statement, byte tableACode, byte tableDCode) : base(statement)
        {
            TableACode = tableACode;
            TableDCode = tableDCode;
        }

        IJoinConfig<Dto, TSource, TOther> IJoinConfig<Dto, TSource, TOther>.And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
            where TKey : default
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinConfig<Dto, TSource, TOther>.CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey)
            where TKey : default
            where TOtherKey : default
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinConfig<Dto, TSource, TOther>.CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey)
            where TKey : default
            where TOtherKey : default
        {
            throw new NotImplementedException();
        }

        IJoinConfig<Dto, TSource, TOther> IJoinConfig<Dto, TSource, TOther>.Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
            where TKey : default
        {
            throw new NotImplementedException();
        }

        IMJoinsProcess<Dto> IJoinConfig<Dto, TSource, TOther>.Then()
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinConfig<Dto, TSource, TOther>.WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinConfig<Dto, TSource, TOther>.WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }

    internal class JoinOptions<Dto, TSource, TOther> : MGroupBy<Dto>, IJoinOptions<Dto, TSource, TOther> where Dto : class
    {
        private readonly byte TableACode;
        private readonly byte TableDCode;

        internal JoinOptions(BHSelectStatementBuilder statement, byte tableACode, byte tableDCode) : base(statement)
        {
            TableACode = tableACode;
            TableDCode = tableDCode;
        }

        IJoinOptions<Dto, TSource, TOther> IJoinOptions<Dto, TSource, TOther>.CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey)
            where TKey : default
            where TOtherKey : default
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinOptions<Dto, TSource, TOther>.CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey)
            where TKey : default
            where TOtherKey : default
        {
            throw new NotImplementedException();
        }

        IMJoinsProcess<Dto> IJoinOptions<Dto, TSource, TOther>.Then()
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinOptions<Dto, TSource, TOther>.WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        IJoinOptions<Dto, TSource, TOther> IJoinOptions<Dto, TSource, TOther>.WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
