using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBHContextBase
    {
        #region Optional Methods
        /// <summary>
        /// 
        /// </summary>
        IBHCommand Command(string commandText, string? databaseIdentity = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBHParameters CreateParameters();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBHTransaction BeginTransaction();
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IBHContext : IBHContextBase
    {
        #region Table Getters
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IBHTable<T> Table<T>() where T : class, BHEntityIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <returns></returns>
        IBHTable<T, G> Table<T, G>() where T : BHEntityAI<G> where G : IComparable<G>;
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHTable<T, G> : IBHTable<T> where T : BHEntityAI<G> where G : IComparable<G>
    {
        #region Unique AI Entity Methods
        /// <summary>
        /// Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction"></param>
        /// <returns>Entity</returns>
        T? GetById(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction"></param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetById<Dto>(G Id, IBHTransaction? transaction = null) where Dto : BHDto<G>;

        /// <summary>
        /// Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        IBHQueryUpdatable<T> UpdateById(T entry, IBHTransaction? transaction = null);

        /// <summary>
        /// Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on the database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="entry">Entity</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        IBHQueryUpdatable<T, Dto> UpdateById<Dto>(Dto entry, IBHTransaction? transaction = null) where Dto : BHDto<G>;


        /// <summary>
        /// Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        IBHQueryUpdatable<T> UpdateById(List<T> entries, IBHTransaction? transaction = null);

        /// <summary>
        /// Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        IBHQueryUpdatable<Dto> UpdateById<Dto>(List<Dto> entries, IBHTransaction? transaction = null) where Dto : BHDto<G>;

        /// <summary>
        /// Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <param name="Id">Entry's Id</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        bool DeleteById(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        bool RemoveById(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        bool RestoreById(G Id, IBHTransaction? transaction = null);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<G> GetIdsWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);
        #endregion

        #region Unique AI Entity Methods Async
        /// <summary>
        /// Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction"></param>
        /// <returns>Entity</returns>
        Task<T?> GetByIdAsync(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction"></param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetByIdAsync<Dto>(G Id, IBHTransaction? transaction = null) where Dto : BHDto<G>;

        /// <summary>
        /// Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <param name="Id">Entry's Id</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        Task<bool> DeleteByIdAsync(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        Task<bool> RemoveByIdAsync(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <param name="transaction"></param>
        /// <returns>Success</returns>
        Task<bool> RestoreByIdAsync(G Id, IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<G>> GetIdsAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHTable<T> : IBHContextBase where T: class, BHEntityIdentifier
    {
        #region Select Methods

        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>All Entities of the Table</returns>
        IBHQuerySearchable<T> Select(IBHTransaction? transaction = null);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        IBHQueryJoinable<Dto, T> Select<Dto>(IBHTransaction? transaction = null) where Dto : BHDto;
        #endregion

        #region Update Methods

        /// <summary>
        /// Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        IBHQueryUpdatable<T> UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction? transaction = null);

        /// <summary>
        /// Finds the entries in the database table
        /// using a Lambda Expression as filter and
        /// uses a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry with the Columns Object's values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Columns Object</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        IBHQueryUpdatable<T, Columns> UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction? transaction = null) where Columns : class;

        #endregion

        // SYNC METHODS

        #region Additional Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Any(IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count(IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);

        #endregion

        #region Insert Methods

        /// <summary>
        /// Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool InsertEntry(T entry, IBHTransaction? transaction = null);

        /// <summary>
        /// Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool InsertEntries(IBHQuery<T> entries, IBHTransaction? transaction = null);
        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes All entires of the database table.
        /// </summary>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteAllEntries(IBHTransaction? transaction = null);

        /// <summary>
        /// Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);

        #endregion

        // ASYNC METHODS

        #region Additional Methods Async

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync(IBHTransaction? transaction = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);

        #endregion

        #region Insert Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntryAsync(T entry, IBHTransaction? transaction = null);

        /// <summary>
        /// <b>Asyncronous.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntriesAsync(IBHQuery<T> entries, IBHTransaction? transaction = null);

        #endregion

        #region Delete Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Deletes All entires of the database table. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync(IBHTransaction? transaction = null);

        /// <summary>
        /// <b>Asyncronous.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction? transaction = null);
        #endregion
    }
}
