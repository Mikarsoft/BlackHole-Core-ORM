using BlackHole.Engine;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.PreLoads
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    public class StoredViewsProcess<Dto> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }
        internal bool IsFirst { get; set; }

        internal StoredViewsProcess()
        {
            Data = new JoinsData(typeof(Dto));
            Data.InitializeOccupiedProperties();
            IsFirst = true;
        }

        internal StoredViewsProcess(JoinsData data)
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
        public PreStoredView<Dto, TSource, TOther> InnerJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "inner", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreStoredView<Dto, TSource, TOther> OuterJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "full outer", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreStoredView<Dto, TSource, TOther> LeftJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "left", IsFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TOther"></typeparam>
        /// <returns></returns>
        public PreStoredView<Dto, TSource, TOther> RightJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "right", IsFirst);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class PreStoredView<Dto, Tsource, TOther> 
        where Tsource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }
        internal string StoredViewType { get; set; }
        internal bool IsFirst { get; set; }

        internal PreStoredView(JoinsData data, string storedViewType, bool isFirst)
        {
            Data = data;
            StoredViewType = storedViewType;
            IsFirst = isFirst;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="key"></param>
        /// <param name="otherkey"></param>
        /// <returns></returns>
        public StoredViewConfig<Dto, Tsource, TOther> On<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.CreateJoin<Dto, Tsource, TOther>(key, otherkey, StoredViewType, IsFirst);
            return new StoredViewConfig<Dto, Tsource, TOther>(Data);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class StoredViewConfig<Dto, Tsource, TOther> 
        where Tsource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal StoredViewConfig(JoinsData data)
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
        public StoredViewConfig<Dto, Tsource, TOther> And<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
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
        public StoredViewConfig<Dto, Tsource, TOther> Or<Tkey>(Expression<Func<Tsource, Tkey?>> key, Expression<Func<TOther, Tkey?>> otherkey)
        {
            Data.Additional<Tsource, TOther>(key, otherkey, "or");
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StoredViewOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new StoredViewOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StoredViewOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new StoredViewOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        public StoredViewOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey, TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<Tsource>(predicate, castOnDto);
            return new StoredViewOptions<Dto, Tsource, TOther>(Data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TOtherkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="castOnDto"></param>
        /// <returns></returns>
        public StoredViewOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey, TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return new StoredViewOptions<Dto, Tsource, TOther>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StoredViewsProcess<Dto> Then()
        {
            return new StoredViewsProcess<Dto>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void StoreAsView()
        {
            new StoredViewComplete<Dto>(Data).StoreAsView();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    /// <typeparam name="Tsource"></typeparam>
    /// <typeparam name="TOther"></typeparam>
    public class StoredViewOptions<Dto, Tsource, TOther>
        where Tsource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal StoredViewOptions(JoinsData data)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StoredViewOptions<Dto, Tsource, TOther> WhereFirst(Expression<Func<Tsource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StoredViewOptions<Dto, Tsource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
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
        public StoredViewOptions<Dto, Tsource, TOther> CastColumnOfFirst<Tkey, TOtherkey>(Expression<Func<Tsource, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
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
        public StoredViewOptions<Dto, Tsource, TOther> CastColumnOfSecond<Tkey, TOtherkey>(Expression<Func<TOther, Tkey?>> predicate, Expression<Func<Dto, TOtherkey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StoredViewsProcess<Dto> Then()
        {
            return new StoredViewsProcess<Dto>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void StoreAsView()
        {
            new StoredViewComplete<Dto>(Data).StoreAsView();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Dto"></typeparam>
    internal class StoredViewComplete<Dto> where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }
        internal StoredViewComplete(JoinsData data)
        {
            Data = data;
        }

        internal void StoreAsView()
        {
            JoinsData? existingJoin = BlackHoleViews.Stored.FirstOrDefault(x => x.DtoType == typeof(Dto));

            if (existingJoin != null)
            {
                BlackHoleViews.Stored.Remove(existingJoin);
            }

            BlackHoleViews.Stored.Add(Data);
        }
    }
}
