using Mikarsoft.BlackHoleCore.Abstractions.Tools;
using Mikarsoft.BlackHoleCore.Connector;
using Mikarsoft.BlackHoleCore.Connector.Enums;
using Mikarsoft.BlackHoleCore.Connector.Statements;
using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{
    internal class BHJoinsProcess<Dto> : IBHJoinsProcess<Dto> where Dto : BHDto
    {
        private readonly JoinStatement<Dto> StatementBuilder;

        internal BHJoinsProcess(JoinStatement<Dto> statement)
        {
            StatementBuilder = statement;
        }

        public IPreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            StatementBuilder.AddPair(nameof(TSource), nameof(TOther), JoinType.Inner);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder);
        }

        public IPreJoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            StatementBuilder.AddPair(nameof(TSource), nameof(TOther), JoinType.Left);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder);
        }

        public IPreJoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            StatementBuilder.AddPair(nameof(TSource), nameof(TOther), JoinType.Outer);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder);
        }

        public IPreJoin<Dto, TSource, TOther> RightJoin<TSource, TOther>()
            where TSource : BHEntity<TSource>
            where TOther : BHEntity<TOther>
        {
            StatementBuilder.AddPair(nameof(TSource), nameof(TOther), JoinType.Right);
            return new PreJoin<Dto, TSource, TOther>(StatementBuilder);
        }
    }

    internal class PreJoin<Dto, TSource, TOther> : IPreJoin<Dto, TSource, TOther> 
        where Dto : BHDto where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>
    {
        private readonly JoinStatement<Dto> StatementBuilder;

        internal PreJoin(JoinStatement<Dto> statement)
        {
            StatementBuilder = statement;
        }

        public IJoinConfig<Dto, TSource, TOther> On<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
        {
            StatementBuilder.AddConnection(key.MemberParse(), otherKey.MemberParse());
            return new JoinConfig<Dto, TSource, TOther>(StatementBuilder);
        }
    }

    internal class JoinConfig<Dto, TSource, TOther> : BHQuery<Dto>, IJoinConfig<Dto, TSource, TOther> 
        where Dto : BHDto where TSource : BHEntity<TSource> where TOther : BHEntity<TOther>
    {
        private readonly JoinStatement<Dto> StatementBuilder;

        internal JoinConfig(JoinStatement<Dto> statement)
        {
            StatementBuilder = statement;
        }

        public IJoinConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
        {
            StatementBuilder.AddConnection(key.MemberParse(), otherKey.MemberParse(), OuterPairType.And);
            return new JoinConfig<Dto, TSource, TOther>(StatementBuilder);
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            throw new NotImplementedException();
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            throw new NotImplementedException();
        }

        public IJoinComplete<Dto> Finally()
        {
            throw new NotImplementedException();
        }

        public IJoinConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
        {
            StatementBuilder.AddConnection(key.MemberParse(), otherKey.MemberParse(), OuterPairType.Or);
            return new JoinConfig<Dto, TSource, TOther>(StatementBuilder);
        }

        public IBHJoinsProcess<Dto> Then()
        {
            throw new NotImplementedException();
        }

        public IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            BHExpressionPart[] parts = predicate.ParseExpression(StatementBuilder.LatestFirstLetter);
            StatementBuilder.AddWhereCase(parts);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            BHExpressionPart[] parts = predicate.ParseExpression(StatementBuilder.LatestSecondLetter);
            StatementBuilder.AddWhereCase(parts);
            return new JoinOptions<Dto,TSource, TOther>(StatementBuilder);
        }
    }

    internal class JoinOptions<Dto, TSource, TOther> : BHQuery<Dto>, IJoinOptions<Dto, TSource, TOther> where Dto : BHDto
    {
        private readonly JoinStatement<Dto> StatementBuilder;

        internal JoinOptions(JoinStatement<Dto> statement)
        {
            StatementBuilder = statement;
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            throw new NotImplementedException();
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            throw new NotImplementedException();
        }

        public IJoinComplete<Dto> Finally()
        {
            throw new NotImplementedException();
        }

        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(StatementBuilder);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            BHExpressionPart[] parts = predicate.ParseExpression(StatementBuilder.LatestFirstLetter);
            StatementBuilder.AddWhereCase(parts);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            BHExpressionPart[] parts = predicate.ParseExpression(StatementBuilder.LatestSecondLetter);
            StatementBuilder.AddWhereCase(parts);
            return new JoinOptions<Dto, TSource, TOther>(StatementBuilder);
        }
    }

    internal class JoinComplete<Dto> : IJoinComplete<Dto> where Dto : BHDto
    {
        public List<Dto> ExecuteQuery()
        {
            throw new NotImplementedException();
        }

        public Task<List<Dto>> ExecuteQueryAsync()
        {
            throw new NotImplementedException();
        }
    }
}
