using BlackHole.Engine;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public IPrejoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
        {
            return new PreJoin<Dto,TSource, TOther>(Data, "inner", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public IPrejoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "full outer", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public IPrejoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "left", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public IPrejoin<Dto, TSource, TOther> RightJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "right", IsFirst);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    internal class PreJoin<Dto, Tsource, TOther> : IPrejoin<Dto, Tsource, TOther> where Dto : BHDtoIdentifier
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        public IJoinConfig<Dto, Tsource, TOther> On<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.CreateJoin<Dto, Tsource, TOther>(key, otherkey, JoinType, IsFirst);
            return new JoinConfig<Dto, Tsource, TOther>(Data);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    internal class JoinConfig<Dto, Tsource, TOther> : IJoinConfig<Dto, Tsource, TOther> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal JoinConfig(JoinsData data)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        public IJoinConfig<Dto, Tsource, TOther> And<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.Additional<Tsource, TOther>(key, otherkey, "and");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        public IJoinConfig<Dto, Tsource, TOther> Or<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.Additional<Tsource,TOther>(key, otherkey,"or");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new JoinOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new JoinOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey,TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<Tsource>(predicate, castOnDto);
            return new JoinOptions<Dto, Tsource, TOther>(Data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey,TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return new JoinOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IJoinComplete<Dto> Finally()
        {
            return new JoinComplete<Dto>(Data);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    internal class JoinOptions<Dto, Tsource, TOther> : IJoinOptions<Dto, Tsource, TOther> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal JoinOptions(JoinsData data)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey, TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<Tsource>(predicate, castOnDto);
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        public IJoinOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey, TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IBHJoinsProcess<Dto> Then()
        {
            return new BHJoinsProcess<Dto>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IJoinComplete<Dto> Finally()
        {
            return new JoinComplete<Dto>(Data);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
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
