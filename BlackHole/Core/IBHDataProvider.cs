using BlackHole.Engine;
using BlackHole.Entities;
using System.Linq.Expressions;

namespace BlackHole.Core
{
    /// <summary>
    /// Makes all the communication between the Database Table and The Specified Entity.
    /// <para>For custom commands, use IBHConnection Interface</para>
    /// </summary>
    /// <typeparam name="T">BlackHoleEntity</typeparam>
    /// <typeparam name="G">The type of Entity's Id</typeparam>
    public interface IBHDataProvider<T, G> where T : BlackHoleEntity<G> where G : IComparable<G>
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
        /// <param name="predicate"></param>
        /// <returns></returns>
        int CountWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns the Id
        /// of the first entry
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Id of the Entry</returns>
        G? GetIdWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entry Ids</returns>
        List<G> GetIdsWhere(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Any(BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Any(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int Count(BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int CountWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns the Id
        /// of the first entry
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entry</returns>
        G? GetIdWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);


        /// <summary>
        /// <b>Transaction.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entry Ids</returns>
        List<G> GetIdsWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);
        #endregion

        #region Select Methods

        /// <summary>
        /// Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries();

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>() where Dto : BlackHoleDto<G>;

        /// <summary>
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return a List of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        List<T> GetAllInactiveEntries();

        /// <summary>
        /// Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns>Entity</returns>
        T? GetEntryById(G Id);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryById<Dto>(G Id) where Dto : BlackHoleDto<G>;

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
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

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
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries(BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>(BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Transaction.</b> In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        List<T> GetAllInactiveEntries(BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Returns the Entity from this Table that has the
        /// specified Id
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        T? GetEntryById(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryById<Dto>(G Id, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns the first one that matches the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression and the Dto properties that match
        /// with the Entity properties. Returns the Dto columns of the first Entry that satisfies these 
        /// filters
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Columns that match with the filters
        /// and the Dto properties
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of DTOs</returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        // WITH ORDER BY

        /// <summary>
        /// Gets all the entries of the specific Table order by keys
        /// and returns a List of Entities 
        /// </summary>
        /// <param name="orderBy">Order by</param>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries(Action<BHOrderBy<T>> orderBy);

        /// <summary>
        /// Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>(Action<BHOrderBy<T>>  orderBy) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return a List of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        List<T> GetAllInactiveEntries(Action<BHOrderBy<T>>  orderBy);

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
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BlackHoleDto<G>;

        // WITH ORDER BY AND TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        List<T> GetAllEntries(Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        List<Dto> GetAllEntries<Dto>(Action<BHOrderBy<T>> orderBy, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Transaction.</b> In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        List<T> GetAllInactiveEntries(Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        T? GetEntryWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Dto? GetEntryWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<T> GetEntriesWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        List<Dto> GetEntriesWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy, BHTransaction transaction) where Dto : BlackHoleDto<G>;
        #endregion

        #region Insert Methods
        /// <summary>
        /// Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        bool InsertEntry(T entry);

        /// <summary>
        /// Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <returns>Ids of the Entities</returns>
        bool InsertEntries(List<T> entries);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entity</returns>
        bool InsertEntry(T entry, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Ids of the Entities</returns>
        bool InsertEntries(List<T> entries, BHTransaction transaction);


        #endregion

        #region Update Methods

        /// <summary>
        /// Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        bool UpdateEntryById(T entry);

        /// <summary>
        /// Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on the database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        bool UpdateEntryById<Columns>(T entry) where Columns : class;

        /// <summary>
        /// Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Success</returns>
        bool UpdateEntriesById(List<T> entries);

        /// <summary>
        /// Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        /// <returns>Success</returns>
        bool UpdateEntriesById<Columns>(List<T> entries) where Columns : class;

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
        /// <b>Transaction.</b> Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool UpdateEntryById(T entry, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b>Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on the database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool UpdateEntryById<Columns>(T entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// <b>Transaction.</b> Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool UpdateEntriesById(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool UpdateEntriesById<Columns>(List<T> entries, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// <b>Transaction.</b>Finds the entries in the table
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

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <returns>Success</returns>
        bool DeleteAllEntries();

        /// <summary>
        /// Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <param name="Id">Entry's Id</param>
        /// <returns>Success</returns>
        bool DeleteEntryById(G Id);

        /// <summary>
        /// If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <returns>Success</returns>
        bool DeleteInactiveEntryById(G Id);

        /// <summary>
        /// <b>Transaction.</b>Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <returns>Success</returns>
        bool ReactivateEntryById(G Id);

        /// <summary>
        /// Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate);
        
        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <returns>Success</returns>
        bool DeleteAllEntries(BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b>Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <param name="Id">Entry's Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteEntryById(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteInactiveEntryById(G Id, BHTransaction transaction);

        /// <summary>
        /// Activates again an Inactive Entry
        /// in the database.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool ReactivateEntryById(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b>Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        bool DeleteEntriesWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

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
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Asyncronous.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns the Id
        /// of the first entry
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Id of the Entry</returns>
        Task<G?> GetIdAsyncWhere(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <b>Asyncronous.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>List of Entry Ids</returns>
        Task<List<G>> GetIdsAsyncWhere(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<int> CountAsync(BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns the Id
        /// of the first entry
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entry</returns>
        Task<G?> GetIdAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the active entries of the database table that
        /// match with the Lambda Expression filters and returns their Ids
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entry Ids</returns>
        Task<List<G>> GetIdsAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

        #endregion

        #region Select Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>All Active Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync();

        /// <summary>
        /// <b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>() where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Asyncronous.</b> In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return a List of the Inactive Entries
        /// in this Table
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>All Incative Entities of the Table</returns>
        Task<List<T>> GetAllInactiveEntriesAsync();

        /// <summary>
        /// <b>Asyncronous.</b> Returns the Entity from this Table that has the
        /// specified Id
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryByIdAsync(G Id);

        /// <summary>
        /// <b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryByIdAsync<Dto>(G Id) where Dto : BlackHoleDto<G>;

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
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

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
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate) where Dto : BlackHoleDto<G>;

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> Gets all the entries of the specific Table
        /// and returns a List of Entities
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>All Active Entities of the Table</returns>
        Task<List<T>> GetAllEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a List of the Dto.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data transfer Object</typeparam>
        /// <param name="transaction"></param>
        /// <returns>All Active Entities of the Table mapped to DTO</returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> In case you are using the 'UseActivator' Attribute on the Entity
        /// this method will return an IList of the Inactive Entries
        /// in this Table
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>All Incative Entities of the Table</returns>
        Task<List<T>> GetAllInactiveEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Returns the Entity from this Table that has the
        /// specified Id
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Entity</returns>
        Task<T?> GetEntryByIdAsync(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Selects only the columns of the specified Dto that exist on the Table
        /// and returns a Dto of the Entity with the specified Id.
        /// <para>Only the properties of the Dto that have the same name and type with 
        /// some properties of the Entity will be returned. Unmatched properties will be null</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Dto">Data Transfer Object</typeparam>
        /// <param name="Id">Specified Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Data Transfer Object</returns>
        Task<Dto?> GetEntryByIdAsync<Dto>(G Id, BHTransaction transaction) where Dto : BlackHoleDto<G>;

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
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> Generates an Sql command using the Lambda Expression, that filters the
        /// Entries of the table and returns all Entries that match the filters
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>List of Entities</returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);

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
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, BHTransaction transaction) where Dto : BlackHoleDto<G>;

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
        Task<List<Dto>> GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<T>> GetAllInactiveEntriesAsync(Action<BHOrderBy<T>> orderBy);

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
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy) where Dto : BlackHoleDto<G>;

        // WITH ORDER BY AND TRANSACTION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<T>> GetAllEntriesAsync(Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<Dto>> GetAllEntriesAsync<Dto>(Action<BHOrderBy<T>> orderBy, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<T>> GetAllInactiveEntriesAsync(Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<T?> GetEntryAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<Dto?> GetEntryAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<T>> GetEntriesAsyncWhere(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>> orderBy, BHTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Dto"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<List<Dto>> GetEntriesAsyncWhere<Dto>(Expression<Func<T, bool>> predicate, Action<BHOrderBy<T>>  orderBy, BHTransaction transaction) where Dto : BlackHoleDto<G>;

        #endregion

        #region Insert Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Id of the Entity</returns>
        Task<bool> InsertEntryAsync(T entry);

        /// <summary>
        /// <b>Asyncronous.</b> Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Ids of the Entities</returns>
        Task<bool> InsertEntriesAsync(List<T> entries);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Inserts the Entity into the table, generates a new Id 
        /// and returns the Id
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Id of the Entity</returns>
        Task<bool> InsertEntryAsync(T entry, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Inserts a list of Entities into the table, generates a new Id of each one
        /// and returns the list of Ids
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Ids of the Entities</returns>
        Task<bool> InsertEntriesAsync(List<T> entries, BHTransaction transaction);
        #endregion

        #region Update Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntryByIdAsync(T entry);

        /// <summary>
        /// <b>Asyncronous.</b> Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on the database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntryByIdAsync<Columns>(T entry) where Columns : class;

        /// <summary>
        /// <b>Asyncronous.</b> Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesByIdAsync(List<T> entries);

        /// <summary>
        /// <b>Asyncronous.</b> Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesByIdAsync<Columns>(List<T> entries) where Columns : class;

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
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the entry in the table that has
        /// the same Id with the input's Entity and updates all
        /// the columns based on the Entity's property values. 
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntryByIdAsync(T entry, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the entry in the database table that
        /// has the same Id with the input's Entity and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on the database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entry">Entity</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntryByIdAsync<Columns>(T entry, BHTransaction transaction) where Columns : class;

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the entries in the table that have
        /// the same Id with the input's Entities and updates all
        /// the columns based on each Entity's property values
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesByIdAsync(List<T> entries, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the entries in the database table that
        /// has the same Id with the input's Entities and
        /// using a 'Columns' class that has properties with the same
        /// name and type with some properties of the Entity, to specifically update
        /// these columns on each database entry.
        /// <para><b>Important</b> => Primary Key Columns Will NOT be updated</para>
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <typeparam name="Columns">Class with Properties that match with some of the Entity's properties</typeparam>
        /// <param name="entries">List of Entities</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> UpdateEntriesByIdAsync<Columns>(List<T> entries, BHTransaction transaction) where Columns : class;

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
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds the entries in the database table
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

        #endregion

        #region Delete Methods Async

        /// <summary>
        /// <b>Asyncronous.</b> Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync();

        /// <summary>
        /// <b>Asyncronous.</b> Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Entry's Id</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntryByIdAsync(G Id);

        /// <summary>
        /// <b>Asyncronous.</b> If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <returns>Success</returns>
        Task<bool> DeleteInactiveEntryByIdAsync(G Id);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Activates again an Inactive Entry
        /// in the database.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Entity's Id</param>
        /// <returns>Success</returns>
        Task<bool> ReactivateEntryByIdAsync(G Id);

        /// <summary>
        /// <b>Asyncronous.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate);

        // WITH TRANSACTION

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Deletes All entires of the database table.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <returns>Success</returns>
        Task<bool> DeleteAllEntriesAsync(BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b><b>Asyncronous.</b> Finds and deletes the entry of the database table
        /// that has the same Id as the input.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entry gets deactivated instead of deleted and it can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Entry's Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntryByIdAsync(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> If you are using a 'UseActivator' Attribute on this Entity
        /// It finds the entry in the database table that is Inactive and has the same
        /// Id as the input and permanently deletes it from the database.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Inactive Entry's Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteInactiveEntryByIdAsync(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Asyncronous.</b> Activates again an Inactive Entry
        /// in the database.
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="Id">Entity's Id</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> ReactivateEntryByIdAsync(G Id, BHTransaction transaction);

        /// <summary>
        /// <b>Transaction.</b> <b>Asyncronous.</b> Finds and deletes the entries of the database table
        /// that match with the Lambda Expression filters.
        /// If you are using a 'UseActivator' Attribute on this Entity,
        /// the entries get deactivated instead of deleted and they can only
        /// be accessed with the 'GetInactiveEntries' command. 
        /// <para><b>Important</b> => You must use 'await' operator if your next operation depends on this operation</para>
        /// </summary>
        /// <param name="predicate">Lambda Expression</param>
        /// <param name="transaction">Transaction Object</param>
        /// <returns>Success</returns>
        Task<bool> DeleteEntriesAsyncWhere(Expression<Func<T, bool>> predicate, BHTransaction transaction);
        #endregion
    }
}
