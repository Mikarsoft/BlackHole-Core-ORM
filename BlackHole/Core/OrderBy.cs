﻿using BlackHole.Identifiers;

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
        public static BlackHoleOrderBy<T> Ascending<T>(Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> Descending<T>(Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> ThenByAscending<T>(this BlackHoleOrderBy<T> orderBy, Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BlackHoleOrderBy<T> ThenByDescending<T>(this BlackHoleOrderBy<T> orderBy, Func<T, object?> action) where T : IBHEntityIdentifier
        {
            return new();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static BlackHoleLimiter<T> TakeAll<T>(this BlackHoleOrderBy<T> orderBy)
        {
            return new();
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
            return new();
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
            return new();
        }
    }
}
