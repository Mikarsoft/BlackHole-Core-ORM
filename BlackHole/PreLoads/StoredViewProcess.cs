using BlackHole.Engine;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.PreLoads
{
    internal class StoredViewsProcess<Dto> : IStoredViewsProcess<Dto> where Dto : BHDtoIdentifier
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

        public IPreStoredView<Dto, TSource, TOther> InnerJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "inner", IsFirst);
        }

        public IPreStoredView<Dto, TSource, TOther> OuterJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "full outer", IsFirst);
        }

        public IPreStoredView<Dto, TSource, TOther> LeftJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "left", IsFirst);
        }

        public IPreStoredView<Dto, TSource, TOther> RightJoin<TSource, TOther>() 
            where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier
        {
            return new PreStoredView<Dto, TSource, TOther>(Data, "right", IsFirst);
        }
    }

    internal class PreStoredView<Dto, Tsource, TOther> : IPreStoredView<Dto, Tsource, TOther>
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

        public IStoredViewConfig<Dto, Tsource, TOther> On<TKey>(Expression<Func<Tsource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
        {
            Data.CreateJoin<Dto, Tsource, TOther>(key, otherKey, StoredViewType, IsFirst);
            return new StoredViewConfig<Dto, Tsource, TOther>(Data);
        }
    }


    internal class StoredViewConfig<Dto, TSource, TOther> : IStoredViewConfig<Dto, TSource, TOther>
        where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal StoredViewConfig(JoinsData data)
        {
            Data = data;
        }

        public IStoredViewConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
        {
            Data.Additional<TSource, TOther>(key, otherKey, "and");
            return this;
        }

        public IStoredViewConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey)
        {
            Data.Additional<TSource, TOther>(key, otherKey, "or");
            return this;
        }

        public IStoredViewOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new StoredViewOptions<Dto, TSource, TOther>(Data);
        }

        public IStoredViewOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return new StoredViewOptions<Dto, TSource, TOther>(Data);
        }

        public IStoredViewOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TSource>(predicate, castOnDto);
            return new StoredViewOptions<Dto, TSource, TOther>(Data);
        }

        public IStoredViewOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return new StoredViewOptions<Dto, TSource, TOther>(Data);
        }

        public IStoredViewsProcess<Dto> Then()
        {
            return new StoredViewsProcess<Dto>(Data);
        }

        public void StoreAsView()
        {
            new StoredViewComplete<Dto>(Data).StoreAsView();
        }
    }

    internal class StoredViewOptions<Dto, TSource, TOther> : IStoredViewOptions<Dto, TSource, TOther>
        where TSource : BHEntityIdentifier where TOther : BHEntityIdentifier where Dto : BHDtoIdentifier
    {
        internal JoinsData Data { get; set; }

        internal StoredViewOptions(JoinsData data)
        {
            Data = data;
        }

        public IStoredViewOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        public IStoredViewOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate)
        {
            Data.WhereJoin(predicate);
            return this;
        }

        public IStoredViewOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TSource>(predicate, castOnDto);
            return this;
        }

        public IStoredViewOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> predicate, Expression<Func<Dto, TOtherKey?>> castOnDto)
        {
            Data.CastColumn<TOther>(predicate, castOnDto);
            return this;
        }


        public IStoredViewsProcess<Dto> Then()
        {
            return new StoredViewsProcess<Dto>(Data);
        }


        public void StoreAsView()
        {
            new StoredViewComplete<Dto>(Data).StoreAsView();
        }
    }

    internal class StoredViewComplete<Dto>
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
