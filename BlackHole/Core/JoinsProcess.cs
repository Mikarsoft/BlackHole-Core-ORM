using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public class JoinsProcess<Dto>
    {
        internal JoinsProcess() { }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>()
        {
            return new PreJoin<Dto,TSource, TOther>();
        }
    }

    public class PreJoin<Dto, Tsource, TOther>
    {
        public JoinConfig<Dto, Tsource, TOther> On<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            return new JoinConfig<Dto, Tsource, TOther> { };
        }
    }

    public class JoinConfig<Dto, Tsource, TOther>
    {
        public JoinConfig<Dto, Tsource, TOther> And<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            return this;
        }

        public JoinConfig<Dto, Tsource, TOther> Or<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            return this;
        }

        public JoinOptions<Dto, Tsource, TOther> WhereFirst()
        {
            return new JoinOptions<Dto, Tsource, TOther>();
        }

        public JoinOptions<Dto, Tsource, TOther> WhereSecond()
        {
            return new JoinOptions<Dto, Tsource, TOther>();
        }

        public JoinOptions<Dto, Tsource, TOther> CastColumnOfFirst()
        {
            return new JoinOptions<Dto, Tsource, TOther>();
        }

        public JoinOptions<Dto, Tsource, TOther> CastColumnOfSecond()
        {
            return new JoinOptions<Dto, Tsource, TOther>();
        }

        public JoinsProcess<Dto> Then()
        {
            return new JoinsProcess<Dto>();
        }
    }

    public class JoinOptions<Dto, Tsource, TOther>
    {
        public JoinOptions<Dto, Tsource, TOther> WhereFirst()
        {
            return this;
        }

        public JoinOptions<Dto, Tsource, TOther> WhereSecond()
        {
            return this;
        }

        public JoinOptions<Dto, Tsource, TOther> CastColumnOfFirst()
        {
            return this;
        }

        public JoinOptions<Dto, Tsource, TOther> CastColumnOfSecond()
        {
            return this;
        }

        public JoinsProcess<Dto> Then()
        {
            return new JoinsProcess<Dto>();
        }

    }
}
