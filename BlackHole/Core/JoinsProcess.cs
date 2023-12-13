using BlackHole.CoreSupport;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public class JoinsProcess<Dto>
    {
        internal JoinsData Data { get; set; }
        internal bool IsFirst {  get; set; }

        internal JoinsProcess()
        {
            Data = new JoinsData(typeof(Dto));
            Data.InitializeOccupiedProperties();
            IsFirst = true;
        }

        internal JoinsProcess(JoinsData data)
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
        public PreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
        {
            return new PreJoin<Dto,TSource, TOther>(Data, "inner", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreJoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "full outer", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreJoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>()
        {
            return new PreJoin<Dto, TSource, TOther>(Data, "left", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreJoin<Dto, TSource, TOther> RightJoin<TSource, TOther>()
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
    public class PreJoin<Dto, Tsource, TOther>
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
        public JoinConfig<Dto, Tsource, TOther> On<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
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
    public class JoinConfig<Dto, Tsource, TOther>
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
        public JoinConfig<Dto, Tsource, TOther> And<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
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
        public JoinConfig<Dto, Tsource, TOther> Or<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.Additional<Tsource,TOther>(key, otherkey,"or");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public JoinOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new JoinOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public JoinOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
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
        public JoinOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey,TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
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
        public JoinOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey,TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return new JoinOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JoinsProcess<Dto> Then()
        {
            return new JoinsProcess<Dto>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JoinComplete<Dto> Finally()
        {
            return new JoinComplete<Dto> (Data);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class JoinOptions<Dto, Tsource, TOther>
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
        public JoinOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public JoinOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
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
        public JoinOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey, TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
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
        public JoinOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey, TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JoinsProcess<Dto> Then()
        {
            return new JoinsProcess<Dto>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JoinComplete<Dto> Finally()
        {
            return new JoinComplete<Dto>(Data);
        }
    }

    public class JoinComplete<Dto>
    {
        internal JoinsData Data { get; set; }
        internal JoinComplete(JoinsData data)
        {
            Data = data;
        }
    }
}
