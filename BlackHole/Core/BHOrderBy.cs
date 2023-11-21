using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHOrderBy<T>
    {
        internal BlackHoleOrderBy<T> orderBy { get; set; } = new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BlackHoleOrderBy<T> OrderByAscending(Expression<Func<T, object?>> action)
        {
            if(action.Body is MemberExpression mExp)
            {
                orderBy = new BlackHoleOrderBy<T>(mExp.Member.Name, "asc", false);
                return orderBy;
            }

            if(action.Body is UnaryExpression uExp)
            {
                if(uExp.Operand is MemberExpression mExp2)
                {
                    orderBy = new BlackHoleOrderBy<T>(mExp2.Member.Name, "asc", false);
                    return orderBy;
                }
            }

            return orderBy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BlackHoleOrderBy<T> OrderByDescending(Expression<Func<T, object?>> action)
        {
            if (action.Body is MemberExpression mExp)
            {
                orderBy = new BlackHoleOrderBy<T>(mExp.Member.Name, "desc", false);
                return orderBy;
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    orderBy = new BlackHoleOrderBy<T>(mExp2.Member.Name, "desc", false);
                    return orderBy;
                }
            }

            return orderBy;
        } 
    }
}
