

using Mikarsoft.BlackHoleCore.Entities;
using System.Linq.Expressions;

namespace Mikarsoft.BlackHoleCore
{

    public interface IMJoinsProcess<Dto> : IMGroupBy<Dto> where Dto : class
    {
        IPreJoin<Dto, TSource, TOther> InnerJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, TSource, TOther> OuterJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, TSource, TOther> LeftJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();

        IPreJoin<Dto, TSource, TOther> RightJoin<TSource, TOther>() where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new();
    }

    public interface IPreJoin<Dto, TSource, TOther> where Dto : class where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new()
    {
        IJoinConfig<Dto, TSource, TOther> On<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey) where TKey : IComparable;
    }

    public interface IJoinConfig<Dto, TSource, TOther> : IMJoinsProcess<Dto> where Dto : class where TSource : BHEntity<TSource>, new() where TOther : BHEntity<TOther>, new()
    {
        IJoinConfig<Dto, TSource, TOther> And<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

        IJoinConfig<Dto, TSource, TOther> Or<TKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<TOther, TKey?>> otherKey);

        IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);
    }

    public interface IJoinOptions<Dto, TSource, TOther> : IMJoinsProcess<Dto> where Dto : class
    {
        IJoinOptions<Dto, TSource, TOther> WhereFirst(Expression<Func<TSource, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> WhereSecond(Expression<Func<TOther, bool>> predicate);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfFirst<TKey, TOtherKey>(Expression<Func<TSource, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);

        IJoinOptions<Dto, TSource, TOther> CastColumnOfSecond<TKey, TOtherKey>(Expression<Func<TOther, TKey?>> key, Expression<Func<Dto, TOtherKey?>> otherKey);
    }
}
