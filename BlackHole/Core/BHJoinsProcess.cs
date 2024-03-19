using BlackHole.Engine;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal class BHJoinsProcess<Dto> : IBHJoinsProcess<Dto> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }
        internal bool IsFirst {  get; set; }

        internal BHJoinsProcess()
        {
            Data = new JoinsData(typeof(Dto));
            Data.InitializeOccupiedProperties();
            IsFirst = true;
        }

        internal BHJoinsProcess(JoinsData data)
        {
            Data = data;
            IsFirst = false;
        }


        public IPrejoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
        {
            return new PreJoin<Dto,TSource, TOther>(Data, "inner", IsFirst);
        }


        public IPrejoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "full outer", IsFirst);
        }

        public IPrejoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "left", IsFirst);
        }

        public IPrejoin<Dto, TSource, TOther> RightJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "right", IsFirst);
        }
    }


    internal class PreJoin<Dto, TSource, TOther> : IPrejoin<Dto, TSource, TOther> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }
        internal string JoinType { get; set; }
        internal bool IsFirst { get; set; }

        internal PreJoin(JoinsData data, string joinType, bool isFirst)
        {
            Data = data;
            JoinType = joinType;
            IsFirst = isFirst;
        }

        public IJoinConfig<Dto, TSource, TOther> On<Tkey>(Expression<Func<TSource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.CreateJoin<Dto, TSource, TOther>(key, otherkey, JoinType, IsFirst);
            return new JoinConfig<Dto, TSource, TOther>(Data);
        }
    }

    internal class JoinConfig<Dto, TSource, TOther> : IJoinConfig<Dto, TSource, TOther> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal JoinConfig(JoinsData data)
        {
            Data = data;
        }

        public IJoinConfig<Dto, TSource, TOther> And<Tkey>(Expression<Func<TSource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.Additional<TSource, TOther>(key, otherkey, "and");
            return this;
        }

        public IJoinConfig<Dto, TSource, TOther> Or<Tkey>(Expression<Func<TSource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.Additional<TSource,TOther>(key, otherkey,"or");
            return this;
        }

        public IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new JoinOptions<Dto, TSource, TOther>(Data);
        }

        public IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new JoinOptions<Dto, TSource, TOther>(Data);
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey,TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TSource>(predicate, castOnDto);
            return new JoinOptions<Dto, TSource, TOther>(Data);
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<Tkey,TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return new JoinOptions<Dto, TSource, TOther>(Data);
        }

        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(Data);
        }

        public IJoinComplete<Dto> Finally()
        {
            return new JoinComplete<Dto>(Data);
        }
    }

    internal class JoinOptions<Dto, TSource, TOther> : IJoinOptions<Dto, TSource, TOther> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal JoinOptions(JoinsData data)
        {
            Data = data;
        }

        public IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        public IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TSource>(predicate, castOnDto);
            return this;
        }

        public IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return this;
        }

        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(Data);
        }

        public IJoinComplete<Dto> Finally()
        {
            return new JoinComplete<Dto>(Data);
        }
    }

    internal class JoinComplete<Dto> : IJoinComplete<Dto> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }
        internal JoinComplete(JoinsData data)
        {
            Data = data;
        }

        public List<Dto> ExecuteQuery()
        {
            return Data.ExecuteQuery<Dto>();
        }

        public async Task<List<Dto>> ExecuteQueryAsync()
        {
            return await Data.ExecuteQueryAsync<Dto>();
        }

        public List<Dto> ExecuteQuery(IBHTransaction bhTransaction)
        {
            return Data.ExecuteQuery<Dto>(bhTransaction);
        }

        public async Task<List<Dto>> ExecuteQueryAsync(IBHTransaction bhTransaction)
        {
            return await Data.ExecuteQueryAsync<Dto>(bhTransaction);
        }

        public List<Dto> ExecuteQuery(Action<BHOrderBy<Dto>> orderBy)
        {
            return Data.ExecuteQuery(orderBy);
        }

        public async Task<List<Dto>> ExecuteQueryAsync(Action<BHOrderBy<Dto>> orderBy)
        {
            return await Data.ExecuteQueryAsync(orderBy);
        }

        public List<Dto> ExecuteQuery(Action<BHOrderBy<Dto>> orderBy, IBHTransaction bhTransaction)
        {
            return Data.ExecuteQuery(orderBy, bhTransaction);
        }

        public async Task<List<Dto>> ExecuteQueryAsync(Action<BHOrderBy<Dto>> orderBy, IBHTransaction bhTransaction)
        {
            return await Data.ExecuteQueryAsync(orderBy, bhTransaction);
        }
    }
}
