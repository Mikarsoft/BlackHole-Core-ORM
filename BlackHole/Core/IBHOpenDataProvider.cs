using BlackHole.Entities;
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
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        List<T> GetAllInactiveEntries();

        /// <summary>
        /// Transaction.In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        List<T> GetAllInactiveEntries(BHTransaction transaction);

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
        T? InsertAndReturnEntry(T entry);

        /// <summary>
        /// Transaction.Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entity</returns>
        T? InsertAndReturnEntry(T entry, BHTransaction transaction);

        /// <summary>
        /// Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Ids of the Entities</returns>
        List<T> InsertAndReturnEntries(List<T> entries);

        /// <summary>
        /// Transaction.Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Ids of the Entities</returns>
        List<T> InsertAndReturnEntries(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        bool JustInsertEntry(T entry);

        /// <summary>
        /// Transaction.Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entity</returns>
        bool JustInsertEntry(T entry, BHTransaction transaction);

        /// <summary>
        /// Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Ids of the Entities</returns>
        bool JustInsertEntries(List<T> entries);

        /// <summary>
        /// Transaction.Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Ids of the Entities</returns>
        bool JustInsertEntries(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entry">Entity</param>
        bool UpdateEntryById(T entry);

        /// <summary>
        /// Transaction. Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        bool UpdateEntryById(T entry, BHTransaction transaction);

        /// <summary>
        /// Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on the database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        bool UpdateEntryById<Columns>(T entry) where Columns : class;

        /// <summary>
        /// Transaction.Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on the database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        bool UpdateEntryById<Columns>(T entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entries">List of Entities</param>
        bool UpdateEntriesById(List<T> entries);

        /// <summary>
        /// Transaction.Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        bool UpdateEntriesById(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        bool UpdateEntriesById<Columns>(List<T> entries) where Columns : class;

        /// <summary>
        /// Transaction.Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        bool UpdateEntriesById<Columns>(List<T> entries, BHTransaction transaction) where Columns : class;

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
    }
}
