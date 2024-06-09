

using BlackHole.Core;
using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Abstractions.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="G"></typeparam>
    public interface IBHTable<T, G> : IBHTable<T> where T : BHEntityAI<G> where G : IComparable<G>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBHTable<T> where T: class, BHEntityIdentifier
    {
        // SYNC METHODS

        #region Additional Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbIdentity"></param>
        /// <returns></returns>
        IBHConnection CustomCommand(string? dbIdentity = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IBHTransaction BeginIBHTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Any(IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count(IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        #endregion

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
        bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction? transaction = null);

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
        bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction? transaction = null) where Columns : class;

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
        /// <returns></returns>
        Task<bool> AnyAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync(IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

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

        #region Update Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction? transaction = null);

        /// <summary>
        /// <b>Asyncronous.</b> Finds the entries in the database table
        /// using a Lambda Expression as filter and
        /// uses a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry with the Columns Object's values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Columns Object</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction? transaction = null) where Columns : class;
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
