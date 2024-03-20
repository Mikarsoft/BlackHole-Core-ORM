﻿using System.Linq.Expressions;

namespace BlackHole.Entities
{
    /// <summary>
    /// Builder for Entity's Settings
    /// </summary>
    public class EntityOptionsBuilder<T>
    {
        /// <summary>
        /// Sets property as Primary key.
        /// </summary>
        /// <typeparam name="tKey">Type of the property</typeparam>
        /// <param name="primaryKey">Property of the entity</param>
        /// <returns>Entity Settings Object</returns>
        public EntitySettings<T> SetPrimaryKey<tKey>(Expression<Func<T,tKey>> primaryKey) where tKey : IComparable<tKey>
        {
            EntitySettings<T> settings = new(false);
            if (primaryKey.Body is MemberExpression pkMember)
            {
                settings.PKPropertyNames.Add(pkMember.Member.Name);
                settings.MainPrimaryKey = pkMember.Member.Name;
            }
            return settings;
        }

        /// <summary>
        /// Sets the property as Primary Key with auto increment.
        /// </summary>
        /// <param name="primaryKey">Property of the entity</param>
        /// <param name="HasAutoIncrement">Set to true for auto increment</param>
        /// <returns>Entity Settings Object</returns>
        public EntitySettings<T> SetPrimaryKey(Expression<Func<T, int>> primaryKey, bool HasAutoIncrement)
        {
            EntitySettings<T> settings = new(false);
            if (primaryKey.Body is MemberExpression pkMember)
            {
                settings.PKPropertyNames.Add(pkMember.Member.Name);
                settings.MainPrimaryKey = pkMember.Member.Name;
                settings.HasAutoIncrement = HasAutoIncrement;
            }
            return settings;
        }

        /// <summary>
        /// Sets the property as Primary Key with auto increment.
        /// </summary>
        /// <param name="primaryKey">Property of the entity</param>
        /// <param name="HasAutoIncrement">Set to true for auto increment</param>
        /// <returns>Entity Settings Object</returns>
        public EntitySettings<T> SetPrimaryKey(Expression<Func<T, long>> primaryKey, bool HasAutoIncrement)
        {
            EntitySettings<T> settings = new(false);
            if (primaryKey.Body is MemberExpression pkMember)
            {
                settings.PKPropertyNames.Add(pkMember.Member.Name);
                settings.MainPrimaryKey = pkMember.Member.Name;
                settings.HasAutoIncrement = HasAutoIncrement;
            }
            return settings;
        }

        /// <summary>
        /// Sets the property as Primary Key and Sets autogenerated sequence of uuid on Postgres or SqlServer.
        /// <para><b>Important</b> => The Sequence of Uuid works only on Postgres and SqlServer. On any other database this setting will be ignored.</para>
        /// </summary>
        /// <param name="primaryKey">Property of the entity</param>
        /// <param name="HasSequence">Set to true for Uuid Sequence</param>
        /// <returns>Entity Settings Object</returns>
        public EntitySettings<T> SetPrimaryKey(Expression<Func<T, Guid>> primaryKey, bool HasSequence)
        {
            EntitySettings<T> settings = new(false);
            if (primaryKey.Body is MemberExpression pkMember)
            {
                settings.PKPropertyNames.Add(pkMember.Member.Name);
                settings.MainPrimaryKey = pkMember.Member.Name;
                settings.HasAutoIncrement = HasSequence;
            }
            return settings;
        }

        /// <summary>
        /// Sets property as Primary key and uses the Value Generator to create a new value on the Insert.
        /// </summary>
        /// <typeparam name="tKey"></typeparam>
        /// <param name="primaryKey">Property of the entity</param>
        /// <param name="valueGenerator">class that inherits from IBHValueGenerator</param>
        /// <returns>Entity Settings Object</returns>
        public EntitySettings<T> SetPrimaryKey<tKey>(Expression<Func<T, tKey>> primaryKey, IBHValueGenerator<tKey> valueGenerator) where tKey: IComparable<tKey> 
        {
            EntitySettings<T> settings = new (false);
            if (primaryKey.Body is MemberExpression pkMember)
            {
                settings.PKPropertyNames.Add(pkMember.Member.Name);
                settings.MainPrimaryKey = pkMember.Member.Name;
                settings.AutoGeneratedColumns.Add(new AutoGeneratedProperty { PropertyName = pkMember.Member.Name, Autogenerated = true, Generator = valueGenerator });
            }
            return settings;
        }

        /// <summary>
        /// Locks the entity with No Primary keys.
        /// <para><b>Important</b> => If you use 'CompositeKey' after this method, the Composite Key will be ignored.</para>
        /// </summary>
        /// <returns>Entity Settings Object</returns>
        public EntitySettings<T> NoPrimaryKey()
        {
            return new EntitySettings<T>(true);
        }
    }

    /// <summary>
    /// Open Entity Settings Object
    /// </summary>
    /// <typeparam name="T">Type of Entity</typeparam>
    public class EntitySettings<T>
    {
        /// <summary>
        /// List of Primary Keys of the Entity
        /// </summary>
        public List<string> PKPropertyNames { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        public List<AutoGeneratedProperty> AutoGeneratedColumns { get; set; } = new();

        /// <summary>
        /// The first Primary Key
        /// </summary>
        public string MainPrimaryKey { get; set; } = string.Empty;
        /// <summary>
        /// If the first Primary Key has autoincrement this is set to true
        /// </summary>
        public bool HasAutoIncrement { get; set; }
        internal bool LockedPK { get; set; }

        internal EntitySettings(bool lockedPK)
        {
            LockedPK = lockedPK;
        }

        /// <summary>
        /// A public constructor with Locked Primary Key Settings,
        /// to be used in BlackHole Core ORM Error Handling
        /// <para>DO NOT USE This Constructor. Use the Provided EntityOptionsBuilder to generate an unlocked
        /// instance of this class.</para>
        /// </summary>
        public EntitySettings()
        {
            LockedPK = true;
        }
    }

    /// <summary>
    /// Entity Settings Extension methods.
    /// </summary>
    public static class PKExtensions
    {
        /// <summary>
        /// Sets property as additional Primary key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="tKey"></typeparam>
        /// <param name="settings"></param>
        /// <param name="primaryKey"></param>
        /// <returns>Entity Settings Object</returns>
        public static EntitySettings<T> CompositeKey<T, tKey>(this EntitySettings<T> settings, Expression<Func<T, tKey>> primaryKey) where tKey : IComparable<tKey>
        {
            if (!settings.LockedPK)
            {
                if (primaryKey.Body is MemberExpression pkMember)
                {
                    string? ExistingProp = settings.PKPropertyNames.FirstOrDefault(x => x == pkMember.Member.Name);
                    if (string.IsNullOrEmpty(ExistingProp))
                    {
                        settings.PKPropertyNames.Add(pkMember.Member.Name);
                    }
                }
            }
            return settings;
        }

        /// <summary>
        /// Sets property as additional Primary key and uses the Value Generator to create a new value on the Insert.
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <typeparam name="tKey">Type of Property</typeparam>
        /// <param name="settings">Entity Settings Object</param>
        /// <param name="primaryKey">Property of the Entity</param>
        /// <param name="valueGenerator">class that inherits from IBHValueGenerator</param>
        /// <returns>Entity Settings Object</returns>
        public static EntitySettings<T> CompositeKey<T, tKey>(this EntitySettings<T> settings, Expression<Func<T, tKey>> primaryKey, IBHValueGenerator<tKey> valueGenerator)
            where tKey : IComparable<tKey>
        {
            if (!settings.LockedPK)
            {
                if (primaryKey.Body is MemberExpression pkMember)
                {
                    string? ExistingProp = settings.PKPropertyNames.FirstOrDefault(x => x == pkMember.Member.Name);

                    if (string.IsNullOrEmpty(ExistingProp))
                    {
                        settings.PKPropertyNames.Add(pkMember.Member.Name);
                    }

                    AutoGeneratedProperty? ExistingGenerator = settings.AutoGeneratedColumns.FirstOrDefault(x=>x.PropertyName == pkMember.Member.Name);

                    if(ExistingGenerator != null)
                    {
                        settings.AutoGeneratedColumns.Remove(ExistingGenerator);
                    }

                    settings.AutoGeneratedColumns.Add(new AutoGeneratedProperty { PropertyName = pkMember.Member.Name, Autogenerated = true, Generator = valueGenerator });
                }
            }
            return settings;
        }

        /// <summary>
        /// Binds a value Generator with the column and generated a new value on insert.
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <typeparam name="tKey">Type of Property</typeparam>
        /// <param name="settings">Entity Settings Object</param>
        /// <param name="primaryKey">Property of the Entity</param>
        /// <param name="valueGenerator">class that inherits from IBHValueGenerator</param>
        /// <returns>Entity Settings Object</returns>
        public static EntitySettings<T> AutoGenerate<T, tKey>(this EntitySettings<T> settings, Expression<Func<T, tKey>> primaryKey, IBHValueGenerator<tKey> valueGenerator)
            where tKey : IComparable<tKey>
        {
            if (primaryKey.Body is MemberExpression pkMember)
            {
                AutoGeneratedProperty? ExistingGenerator = settings.AutoGeneratedColumns.FirstOrDefault(x => x.PropertyName == pkMember.Member.Name);

                if (ExistingGenerator != null)
                {
                    settings.AutoGeneratedColumns.Remove(ExistingGenerator);
                }

                settings.AutoGeneratedColumns.Add(new AutoGeneratedProperty { PropertyName = pkMember.Member.Name, Autogenerated = true, Generator = valueGenerator });
            }
            
            return settings;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AutoGeneratedProperty
    {
        internal AutoGeneratedProperty()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public bool Autogenerated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object? Generator { get; set; }
    }
}
