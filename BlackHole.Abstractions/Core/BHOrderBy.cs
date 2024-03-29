﻿using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BHOrderBy<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public BHOrderBy() { }

        /// <summary>
        /// 
        /// </summary>
        public BlackHoleOrderBy<T> OrderBy { get; set; } = new();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public BlackHoleOrderBy<T> OrderByAscending(Expression<Func<T, object?>> action)
        {
            if(action.Body is MemberExpression mExp)
            {
                OrderBy = new BlackHoleOrderBy<T>(mExp.Member.Name, "asc", false);
                return OrderBy;
            }

            if(action.Body is UnaryExpression uExp)
            {
                if(uExp.Operand is MemberExpression mExp2)
                {
                    OrderBy = new BlackHoleOrderBy<T>(mExp2.Member.Name, "asc", false);
                    return OrderBy;
                }
            }

            return OrderBy;
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
                OrderBy = new BlackHoleOrderBy<T>(mExp.Member.Name, "desc", false);
                return OrderBy;
            }

            if (action.Body is UnaryExpression uExp)
            {
                if (uExp.Operand is MemberExpression mExp2)
                {
                    OrderBy = new BlackHoleOrderBy<T>(mExp2.Member.Name, "desc", false);
                    return OrderBy;
                }
            }

            return OrderBy;
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlackHoleOrderBy<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OrderByPair> OrderProperties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LockedByError { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool TakeSpecificRange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FromRow { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int ToRow { get; set; } = 0;

        internal BlackHoleOrderBy()
        {
            OrderProperties = new();
            LockedByError = true;
        }

        internal BlackHoleOrderBy(string propertyName, string orientation, bool lockedByError)
        {
            OrderProperties = new()
            {
                new OrderByPair { Orientation = orientation, PropertyName = propertyName }
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

            if (fetchRows < 1)
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
            if (offsetRows < 0)
            {
                offsetRows = 0;
            }

            if (fetchRows < 1)
            {
                fetchRows = 1;
            }

            FromRow = offsetRows;
            ToRow = fetchRows;

            TakeSpecificRange = true;
        }

        private void AddPair(string propertyName, string orientation)
        {
            OrderProperties.Add(new OrderByPair { Orientation = orientation, PropertyName = propertyName });
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class OrderByPair
    {
        internal OrderByPair()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Orientation { get; set; } = string.Empty;
    }
}
