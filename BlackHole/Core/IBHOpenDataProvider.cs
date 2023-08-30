﻿using BlackHole.CoreSupport;
using BlackHole.Entities;
using BlackHole.Identifiers;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    internal interface IBHOpenDataProvider<T> where T :BHOpenEntity<T>
    {
        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries();

        /// <summary>
        /// Transaction.Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries(BHTransaction transaction);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>() where Dto : class;

        /// <summary>
        /// Transaction.Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>(BHTransaction transaction) where Dto : class;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Generates an Sql command using the Lambda Expression, that filters the
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
        /// Transaction.Generates an Sql command using the Lambda Expression and the Dto properties that match
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
        /// <returns>IList of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>IList of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class;

        /// <summary>
        /// Transaction.Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>IList of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class;

        /// <summary>
        /// Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        bool InsertEntry(T entry);

        /// <summary>
        /// Transaction.Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entity</returns>
        bool InsertEntry(T entry, BHTransaction transaction);

        /// <summary>
        /// Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Ids of the Entities</returns>
        bool InsertEntries(List<T> entries);

        /// <summary>
        /// Transaction.Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Ids of the Entities</returns>
        bool InsertEntries(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry);

        /// <summary>
        /// Transaction.Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <param name="entry">Entity</param>
        bool UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction);

        /// <summary>
        /// Finds the entries in the database table
        /// using a Lambda Expression as filter and
        /// uses a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry with the Columns Object's values.
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Columns Object</param>
        bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class;

        /// <summary>
        /// Transaction.Finds the entries in the database table
        /// using a Lambda Expression as filter and
        /// uses a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry with the Columns Object's values.
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Columns Object</param>
        /// <param name="transaction">Transaction Object</param>
        bool UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        bool DeleteAllEntries();

        /// <summary>
        /// Transaction.Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        bool DeleteAllEntries(BHTransaction transaction);

        /// <summary>
        /// Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync();

        /// <summary>
        /// Transaction.Asyncronous. Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>() where Dto : class;

        /// <summary>
        /// Transaction.Asyncronous. Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(BHTransaction transaction) where Dto : class;

        /// <summary>
        /// Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class;

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class;

        /// <summary>
        /// Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>IList of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties 
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of DTOs</returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : class;

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties 
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>IList of DTOs</returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : class;

        /// <summary>
        /// Asyncronous. Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        Task<bool> InsertEntryAsync(T entry);

        /// <summary>
        /// Transaction.Asyncronous. Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entity</returns>
        Task<bool> InsertEntryAsync(T entry, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Ids of the Entities</returns>
        Task<bool> InsertEntriesAsync(List<T> entries);

        /// <summary>
        /// Transaction.Asyncronous. Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Ids of the Entities</returns>
        Task<bool> InsertEntriesAsync(List<T> entries, BHTransaction transaction);


        /// <summary>
        /// Asyncronous. Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry);

        /// <summary>
        /// Transaction.Asyncronous. Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        Task<bool> UpdateEntriesAsyncWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds the entries in the database table
        /// using a Lambda Expression as filter and
        /// uses a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry with the Columns Object's values.
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Columns Object</param>
        Task<bool> UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class;

        /// <summary>
        /// Transaction.Asyncronous. Finds the entries in the database table
        /// using a Lambda Expression as filter and
        /// uses a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry with the Columns Object's values.
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Columns Object</param>
        /// <param name="transaction">Transaction Object</param>
        Task<bool> UpdateEntriesAsyncWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Asyncronous. Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        Task<bool> DeleteAllEntriesAsync();

        /// <summary>
        /// Transaction.Asyncronous. Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        Task<bool> DeleteAllEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
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
