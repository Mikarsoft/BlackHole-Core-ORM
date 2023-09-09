using BlackHole.CoreSupport;
using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Datbase Table and The Specified Entity
    /// <para>For custom commands, use IBHConnection Interface</para>
    /// </summary>
    /// <typeparam name="T">BHOpenEntity</typeparam>
    public interface IBHOpenDataProvider<T> where T :BHOpenEntity<T>
    {
        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        List<T> GetAllEntries();

        /// <summary>
        /// <b>Transaction.</b>Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        List<T> GetAllEntries(BHTransaction transaction);

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
        /// <b>Transaction.</b>Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>(BHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class;

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        bool InsertEntry(T entry);

        /// <summary>
        /// <b>Transaction.</b> Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool InsertEntry(T entry, BHTransaction transaction);

        /// <summary>
        /// Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Success</returns>
        bool InsertEntries(List<T> entries);

        /// <summary>
        /// <b>Transaction.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool InsertEntries(List<T> entries, BHTransaction transaction);

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
        /// <b>Transaction.</b> Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction);

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
        bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Deletes All entires of the database table.
        /// </summary>
        /// <returns>Success</returns>
        bool DeleteAllEntries();

        /// <summary>
        /// <b>Transaction.</b> Deletes All entires of the database table.
        /// </summary>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteAllEntries(BHTransaction transaction);

        /// <summary>
        /// Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Transaction.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// <b>Asyncronous.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync();

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>All Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync(BHTransaction transaction);

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
        /// <b>Transaction.</b> <b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(BHTransaction transaction) where Dto : BHOpenDto;

        /// <summary>
        /// <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

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
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class;

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
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class;

        /// <summary>
        /// <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

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
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BHOpenDto;

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
        /// <b>Transaction.</b> <b>Asyncronous.</b> Inserts the Entity into the table, and updates its
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on its properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntryAsync(T entry, BHTransaction transaction);

        /// <summary>
        /// <b>Asyncronous.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntriesAsync(List<T> entries);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Inserts a list of Entities into the table, and updates their
        /// values, if there is auto increment or custom IBHValueGenerators,
        /// applied on their properties.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> InsertEntriesAsync(List<T> entries, BHTransaction transaction);

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
        Task<bool> UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction);

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
        Task<bool> UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// <b>Asyncronous.</b> Deletes All entires of the database table. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync();

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Deletes All entires of the database table. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// <b>Asyncronous.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b>Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Starts a Joins sequence, with the first one as 'Inner Join' that can be continued with 
        /// the 'BlackHole.ExtensionMethods' namespace's methods
        /// </summary>
        /// <typeparam name="TOther">The other Table as Entity</typeparam>
        /// <typeparam name="Tkey">The type of their joint column</typeparam>
        /// <typeparam name="Dto">The exported Data Transfer Object Class</typeparam>
        /// <param name="key">Column of this Table</param>
        /// <param name="otherKey">Column of the Other Table</param>
        /// <returns>The Calculated Data of this Joins Part</returns>
        JoinsData<Dto, T, TOther> InnerJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
           where Dto : IBHDtoIdentifier where TOther : IBHEntityIdentifier where Tkey : IComparable;

        /// <summary>
        /// Starts a Joins sequence, with the first one as 'Outer Join' that can be continued with 
        /// the 'BlackHole.ExtensionMethods' namespace's methods
        /// </summary>
        /// <typeparam name="TOther">The other Table as Entity</typeparam>
        /// <typeparam name="Tkey">The type of their joint column</typeparam>
        /// <typeparam name="Dto">The exported Data Transfer Object Class</typeparam>
        /// <param name="key">Column of this Table</param>
        /// <param name="otherKey">Column of the Other Table</param>
        /// <returns>The Calculated Data of this Joins Part</returns>
        JoinsData<Dto, T, TOther> OuterJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
            where Dto : IBHDtoIdentifier where TOther : IBHEntityIdentifier where Tkey : IComparable;

        /// <summary>
        /// Starts a Joins sequence, with the first one as 'Left Join' that can be continued with 
        /// the 'BlackHole.ExtensionMethods' namespace's methods
        /// </summary>
        /// <typeparam name="TOther">The other Table as Entity</typeparam>
        /// <typeparam name="Tkey">The type of their joint column</typeparam>
        /// <typeparam name="Dto">The exported Data Transfer Object Class</typeparam>
        /// <param name="key">Column of this Table</param>
        /// <param name="otherKey">Column of the Other Table</param>
        /// <returns>The Calculated Data of this Joins Part</returns>
        JoinsData<Dto, T, TOther> LeftJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
            where Dto : IBHDtoIdentifier where TOther : IBHEntityIdentifier where Tkey : IComparable;

        /// <summary>
        /// Starts a Joins sequence, with the first one as 'Right Join' that can be continued with 
        /// the 'BlackHole.ExtensionMethods' namespace's methods
        /// </summary>
        /// <typeparam name="TOther">The other Table as Entity</typeparam>
        /// <typeparam name="Tkey">The type of their joint column</typeparam>
        /// <typeparam name="Dto">The exported Data Transfer Object Class</typeparam>
        /// <param name="key">Column of this Table</param>
        /// <param name="otherKey">Column of the Other Table</param>
        /// <returns>The Calculated Data of this Joins Part</returns>
        JoinsData<Dto, T, TOther> RightJoin<TOther, Tkey, Dto>(Expression<Func<T, Tkey>> key, Expression<Func<TOther, Tkey>> otherKey)
           where Dto : IBHDtoIdentifier where TOther : IBHEntityIdentifier where Tkey : IComparable;
    }
}
