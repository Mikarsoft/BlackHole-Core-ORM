using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Database Table and The Specified Entity
    /// <para>For custom commands, use IBHConnection Interface</para>
    /// </summary>
    /// <typeparam name="T">BHOpenEntity</typeparam>
    public interface IBHOpenDataProvider<T> where T : BHOpenEntity<T>
    {
        // SYNC METHODS

        #region Additional Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Any();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int CountWhere(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

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
        int CountWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        #endregion

        #region Select Methods

        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        List<T> GetAllEntries();

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>() where Dto : BHOpenDto;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BHOpenDto;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BHOpenDto;

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b>Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        List<T> GetAllEntries(IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b>Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>(IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction transaction) where Dto : BHOpenDto;

        // WITH ORDER BY

        /// <summary>
        /// Gets all the entries of the specific Table order by keys
        /// and returns a List of Entities 
        /// </summary>
        /// <param name="orderBy">Order by</param>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries(Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        List<Dto> GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BHOpenDto;

        // WITH ORDER BY AND TRANSACTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<T> GetAllEntries(Action<BHOrderBy<T>> orderBy, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<Dto> GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy, IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction) where Dto : BHOpenDto;

        #endregion

        #region Insert Methods

        /// <summary>
        /// Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        bool InsertEntry(T entry);

        /// <summary>
        /// Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Success</returns>
        bool InsertEntries(List<T> entries);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool InsertEntry(T entry, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool InsertEntries(List<T> entries, IBHTransaction transaction);
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
        /// <returns>Success</returns>
        bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry);

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
        /// <returns>Success</returns>
        bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class;

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Finds the entries in the database table
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
        bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction transaction) where Columns : class;

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes All entires of the database table.
        /// </summary>
        /// <returns>Success</returns>
        bool DeleteAllEntries();

        /// <summary>
        /// Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Deletes All entires of the database table.
        /// </summary>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteAllEntries(IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

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

        #region Select Methods Async
        /// <summary>
        /// <b>Asyncronous.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync();

        /// <summary>
        /// <b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>() where Dto : BHOpenDto;

        /// <summary>
        /// <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties 
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of DTOs</returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BHOpenDto;

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync(IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties 
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of DTOs</returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, IBHTransaction transaction) where Dto : BHOpenDto;

        // WITH ORDER BY

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<T>> GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BHOpenDto;

        // WITH ORDER BY AND TRANSACTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<T>> GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy, IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, IBHTransaction transaction) where Dto : BHOpenDto;
        #endregion

        #region Insert Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntryAsync(T entry);

        /// <summary>
        /// <b>Asyncronous.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntriesAsync(List<T> entries);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntryAsync(T entry, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntriesAsync(List<T> entries, IBHTransaction transaction);

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
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry);

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
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class;

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b>  Finds the entries in the database table
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
        Task<bool> UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, IBHTransaction transaction) where Columns : class;
        #endregion

        #region Delete Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Deletes All entires of the database table. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync();

        /// <summary>
        /// <b>Asyncronous.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Deletes All entires of the database table. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync(IBHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b>Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, IBHTransaction transaction);
        #endregion
    }
}
