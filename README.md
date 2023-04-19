# BlackHole-Core-ORM
A fully automated, very easy to use and setup, with many new features, Object Relational Mapping Library, for .Net Core 6 and 7. Using Custom Data Providers that are 3x times faster than EF Core and Dapper. Many interfaces for easy Reading and Writing Data. Extention Methods for Joining any tables.
It Supports SQL SERVER, MYSQL, POSTGRESQL, ORACLE and SQLITE.

- 6.0.0  is released.
  - changes => 
  
   - Replaced Dapper with Custom Mappers that are 3 times faster
   
   - Added more Methods to the BHDataProvider
   
   - Added IBHConnection Interface for custom commands.
   
   - Improved Configuration made it simpler with more options
   
   - Added Logs Cleaner to automatically clean up the logs that are aged more than x days.
   
   - Added Support for Oracle Database
   
   - Added developer mode that allows dropping columns and constraints on entity changes. But when it's set to false,
     protects the production database from data loss, by disabling columns dropping. And instead it makes them
     nullable, if the property of the entity has been deleted.
     
   - Added BHParameters for custom queries and commands.
   
   - Improved transaction adding RoleBack option and performing automatic Rollback if the transaction fails.
   
   - Added configuration option to load Entities and Services from selected Namespaces or Other Assemblies.
     This way you can make your Apps transform into different App, depending on the configuration.
     
   - Code clean up and redesign. Organized namespaces better for more efficiency. 
     The user now has all the configuration Items into 'BlackHole.Configuration' namespace.
     All the Functionalities into 'BlackHole.Core' namespace.
     And all the Attributes for the entities into the 'BlackHole.Entities' namespace.
    
   *note BlackHole can Not install oracle database to your system. You need to have an instance installed and BlackHole can 
    only create or drop tables in this type of database.




An Example Project is coming soon with the Documentation here => [BlackHole Example](https://github.com/Mikarsoft/BlackHole-Example-Project)

I am still working on the documentation but here are some quick steps to use this ORM

- In Your project install Black.Hole.ORM from nuget

- In your Program.cs add 'using BlackHole.Configuration' and 'using BlackHole.Enums'

- Create some Entities in any folder that Inherit from the class 'BlackHoleEntity<int>' for Entities that are using Integer as Id,
  or 'BlackHoleEntity<Guid>' for Entities that are using Guid as Id
  or 'BlackHoleEntity<string>' for Entities that are using SHA1 hash as Id. (using BlackHole.Entities)

- Add properties on your Entities except the Id property that already exists in the BlackHoleEntity class.

- Add in your entities 'using BlackHole.Attributes.ColumnAttributes' 
  to be able to use '[ForeignKey(typeof(Entity))]' , '[NotNullable]' and '[VarCharSize(int)]' attributes on your Entity's properties
  * You can also use 'UseActivator' attribute on your Entity, to take advantage of the 'IsActive' column in case you need to keep the
  data after delete.

- Make your services Inherit from 'BlackHoleScoped' or 'BlackHoleSingleton' or 'BlackHoleTransient' so they will be automatically
  registered on the Startup without needing services.Add(<>). (using BlackHole.Entities)

- On your IServiceCollection 'services' 
  add services.SuperNova(new BlackHoleBaseConfig{ string ConnectionString, enum BHSqlTypes.SqlType, string LogsPath });
  *The database name in the connection string is your choice. Just make sure that the server of the sql exists*
  *if you don't have any Postgres or MySql or Microsoft Sql Server setup, you can use SqLite with a more simple command =>
  services.SuperNovaLite(string databaseName);
  
 - Last step , go to your services or your controllers and add the Interfaces for the DataProviders =>
  private readonly BHDataProvider<Entity,IdType> _entityService; => Example BHDataProvider<Customer,Guid> _customerService;
  
 - Done! You are ready to use all the functionality of the Data Providers in your services.
  * there are descriptions on the methods of what they do
   
   *The Ids are created automatically on Insert and they get returned.
   * The cascade on Foreign keys is automatic and it depends on the Nullability of the column
   
 I will soon upload here a more detailed guide of all the functionalities of this ORM, such as Stored Views and Joins
 and automatic mapping on DTOs or Updating Specific Columns.
