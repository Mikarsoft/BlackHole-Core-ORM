using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Interfaces
{
    public interface IBHDataProvider<T,G> where T : BlackHoleEntity<G>
    {
        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        IList<T> GetAllEntries();

        /// <summary>
        /// Transaction.Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        IList<T> GetAllEntries(BHTransaction transaction);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        IList<Dto> GetAllEntries<Dto>() where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        IList<Dto> GetAllEntries<Dto>(BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        IList<T> GetAllInactiveEntries();

        /// <summary>
        /// Transaction.In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        IList<T> GetAllInactiveEntries(BHTransaction transaction);

        /// <summary>
        /// Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns>Entity</returns>
        T? GetEntryById(G Id);

        /// <summary>
        /// Transaction.Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns>Entity</returns>
        T? GetEntryById(G Id, BHTransaction transaction);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryById<Dto>(G Id) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryById<Dto>(G Id, BHTransaction transaction) where Dto : BlackHoleDto<G>;

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
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate,BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of Entities</returns>
        IList<T> GetEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of Entities</returns>
        IList<T> GetEntriesWhere(Expression<Func<T, bool>> predicate,BHTransaction transaction);

        /// <summary>
        /// Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of DTOs</returns>
        IList<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of DTOs</returns>
        IList<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate,BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        G? InsertEntry(T entry);

        /// <summary>
        /// Transaction.Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        G? InsertEntry(T entry, BHTransaction transaction);

        /// <summary>
        /// Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Ids of the Entities</returns>
        List<G?> InsertEntries(List<T> entries);

        /// <summary>
        /// Transaction.Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Ids of the Entities</returns>
        List<G?> InsertEntries(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        Task<IList<T>> GetAllEntriesAsync();

        /// <summary>
        /// Transaction.Asyncronous. Gets all the entries of the specific Table
        /// and returns an IList of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        Task<IList<T>> GetAllEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        Task<IList<Dto>> GetAllEntriesAsync<Dto>() where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Asyncronous. Selects only the columns of the specified Dto that exist on the Table
        /// and returns an IList of the Dto.
        /// Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        Task<IList<Dto>> GetAllEntriesAsync<Dto>(BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Asyncronous. In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        Task<IList<T>> GetAllInactiveEntriesAsync();

        /// <summary>
        /// Transaction.Asyncronous. In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        Task<IList<T>> GetAllInactiveEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryByIdAsync(G Id);

        /// <summary>
        /// Transaction.Asyncronous. Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryByIdAsync(G Id, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryByIdAsync<Dto>(G Id) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Asyncronous. Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryByIdAsync<Dto>(G Id, BHTransaction transaction) where Dto : BlackHoleDto<G>;

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
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of Entities</returns>
        Task<IList<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of Entities</returns>
        Task<IList<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties 
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of DTOs</returns>
        Task<IList<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Transaction.Asyncronous. Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties 
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>IList of DTOs</returns>
        Task<IList<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// Asyncronous. Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        Task<G?> InsertEntryAsync(T entry);

        /// <summary>
        /// Transaction.Asyncronous. Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        Task<G?> InsertEntryAsync(T entry, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Ids of the Entities</returns>
        Task<List<G?>> InsertEntriesAsync(List<T> entries);

        /// <summary>
        /// Transaction.Asyncronous. Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Ids of the Entities</returns>
        Task<List<G?>> InsertEntriesAsync(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntryById(T entry);

        /// <summary>
        /// Transaction.Asyncronous. Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntryById(T entry, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on the database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntryById<Columns>(T entry) where Columns : class;

        /// <summary>
        /// Transaction.Asyncronous. Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on the database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntryById<Columns>(T entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Asyncronous. Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entries">List of Entities</param>
        Task<bool> UpdateEntriesById(List<T> entries);

        /// <summary>
        /// Transaction.Asyncronous. Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="entries">List of Entities</param>
        Task<bool> UpdateEntriesById(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        Task<bool> UpdateEntriesById<Columns>(List<T> entries) where Columns : class;

        /// <summary>
        /// Transaction.Asyncronous. Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specificaly update
        /// these columns on each database entry. !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        Task<bool> UpdateEntriesById<Columns>(List<T> entries, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Asyncronous. Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry);

        /// <summary>
        /// Transaction.Asyncronous. Finds the entries in the table
        /// using a Lambda Expression as filter and updates all
        /// the columns based on the inserted Entity's property values. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="entry">Entity</param>
        Task<bool> UpdateEntriesWhere(Expression<Func<T, bool>> predicate, T entry, BHTransaction transaction);

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
        Task<bool> UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry) where Columns : class;

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
        Task<bool> UpdateEntriesWhere<Columns>(Expression<Func<T, bool>> predicate, Columns entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// Asyncronous. Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        Task<bool> DeleteAllEntries();

        /// <summary>
        /// Transaction.Asyncronous. Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        Task<bool> DeleteAllEntries(BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="id">Entry's Id</param>
        Task<bool> DeleteEntryById(G id);

        /// <summary>
        /// Transaction.Asyncronous. Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="id">Entry's Id</param>
        Task<bool> DeleteEntryById(G id, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <param name="id">Inactive Entry's Id</param>
        Task<bool> DeleteInactiveEntryById(G id);

        /// <summary>
        /// Transaction.Asyncronous. If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <param name="id">Inactive Entry's Id</param>
        Task<bool> DeleteInactiveEntryById(G id, BHTransaction transaction);

        /// <summary>
        /// Transaction.Asyncronous. Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ReactivateEntryById(G id);

        /// <summary>
        /// Asyncronous. Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> ReactivateEntryById(G id, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        Task<bool> DeleteEntriesWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// !!Important!! => You must use 'await' operator if your next operation depends on this operation
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        Task<bool> DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns the Id
        /// of the first entry
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Id of the Entry</returns>
        Task<G?> GetIdWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns the Id
        /// of the first entry
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Id of the Entry</returns>
        Task<G?> GetIdWhereAsync(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// Asyncronous. Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entry Ids</returns>
        Task<List<G>> GetIdsWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Transaction.Asyncronous. Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entry Ids</returns>
        Task<List<G>> GetIdsWhereAsync(Expression<Func<T, bool>> predicate, BHTransaction transaction);

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
           where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;

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
            where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;

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
            where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;

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
           where Dto : BlackHoleDto where TOther : BlackHoleEntity where Tkey : IComparable;
    }
}
