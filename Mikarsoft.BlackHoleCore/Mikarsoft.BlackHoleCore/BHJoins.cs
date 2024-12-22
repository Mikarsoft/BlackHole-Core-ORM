using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Entities;
using Mikarsoft.BlackHoleCore.Tools;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHJoinsProcess<Dto> : IBHJoinsProcess<Dto> where Dto : BHDto
    {
        private readonly BHStatementBuilder StatementBuilder;

        internal BHJoinsProcess(BHStatementBuilder statement)
        {
            StatementBuilder = statement;
        }

        public IPreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            byte[]tableLetters = StatementBuilder.AddJoin<TSource,TOther>(JoinType.Inner);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder, tableLetters);
        }

        public IPreJoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            byte[] tableLetters = StatementBuilder.AddJoin<TSource, TOther>(JoinType.Left);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder, tableLetters);
        }

        public IPreJoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            byte[] tableLetters = StatementBuilder.AddJoin<TSource, TOther>(JoinType.Outer);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder, tableLetters);
        }

        public IPreJoin<Dto, TSource, TOther> RightJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            byte[] tableLetters = StatementBuilder.AddJoin<TSource, TOther>(JoinType.Right);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder, tableLetters);
        }
    }

    internal class PreJoin<Dto, TSource, TOther> : IPreJoin<Dto, TSource, TOther> 
        where Dto : BHDto where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>
    {
        private readonly BHStatementBuilder StatementBuilder;
        private readonly byte TableACode;
        private readonly byte TableDCode;

        internal PreJoin(BHStatementBuilder statement, byte[] tableLetters)
        {
            StatementBuilder = statement;
            TableACode = tableLetters[0];
            TableDCode = tableLetters[1];
        }

        public IJoinConfig<Dto, TSource, TOther> On<TKey>(Expression<Func<TSource, TKey?>> key,
            Expression<Func<TOther, TKey?>> otherKey)
        {
            StatementBuilder.AddJoinPoint(key, otherKey);
            return new JoinConfig<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }
    }

    internal class JoinConfig<Dto, TSource, TOther> : BHQuery<Dto>, IJoinConfig<Dto, TSource, TOther> 
        where Dto : BHDto where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>
    {
        private readonly byte TableACode;
        private readonly byte TableDCode;

        internal JoinConfig(BHStatementBuilder statement, byte tableACode, byte tableDCode) : base(statement)
        {
            TableACode = tableACode;
            TableDCode = tableDCode;
        }

        public IJoinConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key,
            Expression<Func<TOther, TKey?>> otherKey)
        {
            StatementBuilder.AddJoinPoint(key, otherKey, OuterPairType.And);
            return new JoinConfig<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key,
            Expression<Func<Dto, TOtherKey?>> otherKey)
        {
            StatementBuilder.AddCastCase(key, otherKey, TableACode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key,
            Expression<Func<Dto, TOtherKey?>> otherKey)
        {
            StatementBuilder.AddCastCase(key, otherKey, TableDCode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IJoinConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key,
            Expression<Func<TOther, TKey?>> otherKey)
        {
            StatementBuilder.AddJoinPoint(key, otherKey, OuterPairType.Or);
            return new JoinConfig<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(StatementBuilder);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            StatementBuilder.AddWhereCase(predicate, TableACode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            StatementBuilder.AddWhereCase(predicate, TableDCode);
            return new JoinOptions<Dto,TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }
    }

    internal class JoinOptions<Dto, TSource, TOther> : BHQuery<Dto>, IJoinOptions<Dto, TSource, TOther> where Dto : BHDto
    {
        private readonly byte TableACode;
        private readonly byte TableDCode;

        internal JoinOptions(BHStatementBuilder statement, byte tableACode, byte tableDCode) : base(statement)
        {
            TableACode = tableACode;
            TableDCode = tableDCode;
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key,
            Expression<Func<Dto, TOtherKey?>> otherKey)
        {
            StatementBuilder.AddCastCase(key, otherKey, TableACode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, 
            Expression<Func<Dto, TOtherKey?>> otherKey)
        {
            StatementBuilder.AddCastCase(key, otherKey, TableDCode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(StatementBuilder);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            StatementBuilder.AddWhereCase(predicate, TableACode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            StatementBuilder.AddWhereCase(predicate, TableDCode);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder, TableACode, TableDCode);
        }
    }
}
