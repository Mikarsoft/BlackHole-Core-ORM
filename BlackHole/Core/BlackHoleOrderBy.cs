
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlackHoleOrderBy<T>
    {
        internal List<OrderByPair> OrderProperties {  get; set; }
        internal bool LockedByError { get; set; }
        internal bool TakeSpecificRange { get; set; }
        internal int FromRow { get; set; } = 0;
        internal int ToRow { get; set; } = 0;

        internal BlackHoleOrderBy()
        {
            OrderProperties = new();
            LockedByError = true;
        }

        internal BlackHoleOrderBy(string propertyName, string orientation, bool lockedByError)
        {
            OrderProperties = new()
            {
                new OrderByPair { Oriantation = orientation, PropertyName = propertyName }
            };

            LockedByError = lockedByError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BlackHoleOrderBy<T> ThenByAscending(Expression<Func<T, object?>> action)
        {
            if (action.Body is MemberExpression mExp)
            {
                AddPair(mExp.Member.Name, "asc");
                return this;
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    AddPair(mExp2.Member.Name, "asc");
                    return this;
                }
            }

            LockedByError = true;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BlackHoleOrderBy<T> ThenByDescending(Expression<Func<T, object?>> action)
        {
            if (action.Body is MemberExpression mExp)
            {
                AddPair(mExp.Member.Name, "desc");
                return this;
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    AddPair(mExp2.Member.Name, "desc");
                    return this;
                }
            }

            LockedByError = true;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchRows"></param>
        public void Take(int fetchRows)
        {
            TakeSpecificRange = true;

            if(fetchRows < 1)
            {
                ToRow = 1;
            }
            else
            {
                ToRow = fetchRows;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetRows"></param>
        /// <param name="fetchRows"></param>
        public void TakeWithOffset(int offsetRows, int fetchRows)
        {
            if(offsetRows < 0)
            {
                offsetRows = 0;
            }

            if(fetchRows < 1)
            {
                fetchRows = 1;
            }

            FromRow = offsetRows;
            ToRow = fetchRows;

            TakeSpecificRange = true;
        }

        private void AddPair(string propertyName, string orientation)
        {
            OrderProperties.Add(new OrderByPair { Oriantation=orientation, PropertyName = propertyName });
        }
    }

    internal class OrderByPair
    {
        internal string PropertyName { get; set; } = string.Empty;
        internal string Oriantation { get; set; } = string.Empty;
    }
}
