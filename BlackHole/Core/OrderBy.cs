using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class OrderBy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> Ascending<T>(this Expression<Func<T, object?>> action) where T:IBHEntityIdentifier
        {
            if(action.Body is MemberExpression mExp)
            {
                return new BlackHoleOrderBy<T>(mExp.Member.Name, "asc", false);
            }

            if(action.Body is UnaryExpression uExp)
            {
                if(uExp.Operand is MemberExpression mExp2)
                {
                    return new BlackHoleOrderBy<T>(mExp2.Member.Name, "asc", false);
                }
            }

            return new("", "", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> Descending<T>(Expression<Func<T, object?>> action) where T : IBHEntityIdentifier
        {
            if (action.Body is MemberExpression mExp)
            {
                return new BlackHoleOrderBy<T>(mExp.Member.Name, "asc", false);
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    return new BlackHoleOrderBy<T>(mExp2.Member.Name, "asc", false);
                }
            }

            return new("", "", true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> ThenByAscending<T>(this BlackHoleOrderBy<T> orderBy, Expression<Func<T, object?>> action) where T : IBHEntityIdentifier
        {
            if (action.Body is MemberExpression mExp)
            {
                orderBy.AddPair(mExp.Member.Name, "asc");
                return orderBy;
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    orderBy.AddPair(mExp2.Member.Name, "asc");
                    return orderBy;
                }
            }

            orderBy.LockedByError = true;
            return orderBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> ThenByDescending<T>(this BlackHoleOrderBy<T> orderBy, Expression<Func<T, object?>> action) where T : IBHEntityIdentifier
        {
            if (action.Body is MemberExpression mExp)
            {
                orderBy.AddPair(mExp.Member.Name, "asc");
                return orderBy;
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    orderBy.AddPair(mExp2.Member.Name, "asc");
                    return orderBy;
                }
            }

            orderBy.LockedByError = true;
            return orderBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static BlackHoleLimiter<T> TakeAll<T>(this BlackHoleOrderBy<T> orderBy)
        {
            return new(orderBy, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="rowsLimit"></param>
        /// <returns></returns>
        public static BlackHoleLimiter<T> Take<T>(this BlackHoleOrderBy<T> orderBy, int rowsLimit)
        {
            return new(orderBy, 0, rowsLimit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="fromRow"></param>
        /// <param name="toRow"></param>
        /// <returns></returns>
        public static BlackHoleLimiter<T> TakeRange<T>(this BlackHoleOrderBy<T> orderBy, int fromRow, int toRow)
        {
            return new(orderBy , fromRow, toRow);
        }
    }
}
