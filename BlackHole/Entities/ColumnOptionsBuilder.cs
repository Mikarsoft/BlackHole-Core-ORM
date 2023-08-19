﻿
using System.Linq.Expressions;

namespace BlackHole.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnOptionsBuilder<T>
    {
        internal List<PrimaryKeySettings> PKSettings { get; set; } = new List<PrimaryKeySettings>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="tKey"></typeparam>
        /// <param name="primaryKey"></param>
        public void SetPrimaryKey<tKey>(Expression<Func<T,tKey>> primaryKey) where tKey : IComparable<tKey>
        {
            if(primaryKey is MemberExpression pkMember)
            {
                PKSettings.Add(new PrimaryKeySettings { PropertyName = pkMember.Member.Name });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="tKey"></typeparam>
        /// <param name="primaryKey"></param>
        /// <param name="valueGenerator"></param>
        public void SetPrimaryKey<tKey>(Expression<Func<T, tKey>> primaryKey, IBHValueGenerator<tKey> valueGenerator) where tKey: IComparable<tKey> 
        {
            if (primaryKey is MemberExpression pkMember)
            {
                PKSettings.Add(new PrimaryKeySettings { PropertyName = pkMember.Member.Name, Autogenerated = true, Generator = valueGenerator });
            }
        }
    }
}
