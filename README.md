# BlackHole-Core-ORM
A fully automated, very easy to use and setup, with many new features, Object Relational Mapping Library, for .Net Core 6 and 7. Using Custom Data Providers that are 3x times faster than EF Core and Dapper. Many interfaces for easy Reading and Writing Data. Extention Methods for Joining any tables.
It Supports SQL SERVER, MYSQL, POSTGRESQL, ORACLE and SQLITE.

6.0.0  is released.

  Changes:
  
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



Example:
 -Find an Example Project here => [BlackHole Example](https://github.com/Mikarsoft/BlackHole-Example-Project)
 -Find Online Documentation here => [BlackHole Documentation] (https://mikarsoft.com/BHDocumentation/index.html)

Quick Start:

- In Your project install Black.Hole.ORM from nuget

- In your Program.cs add 'using BlackHole.Configuration'
  Add the following line into your IServiceCollection =>
    services.SuperNova(settings => settings.IsDeveloperMode(devmode == "dev").AddDatabase(connection => connection.UseOracle(connectionString)))

- Create some Entities in any folder that Inherit from the class 'BlackHoleEntity<int>' for Entities that are using Integer as Id,
  or 'BlackHoleEntity<Guid>' for Entities that are using Guid as Id
  or 'BlackHoleEntity<string>' for Entities that are using SHA1 hash as Id. (using BlackHole.Entities)

- Add properties on your Entities except the Id property that already exists in the BlackHoleEntity class.

- Add Attributes to the properties of your Entities' 
  '[ForeignKey(typeof(Entity), nullability)]' , '[NotNullable]' and '[VarCharSize(int)]'
  * You can also use '[UseActivator]' attribute on your Entity, to take advantage of the 'IsActive' column in case you need to keep the
  data after delete.

- Make your services Inherit from 'BlackHoleScoped' or 'BlackHoleSingleton' or 'BlackHoleTransient' so they will be automatically
  registered on the Startup without needing services.Add(<>). (using BlackHole.Services)
  
 - Last step , go to your services or your controllers and add the Interfaces for the DataProviders =>
  private readonly IBHDataProvider<Entity,IdType> _entityService;
  Example: IBHDataProvider<Customer,Guid> _customerService; (using BlackHole.Core)
 
 - For custom queries and commands, use the IBHConnection Interface that is already Injected with Dependency Injection.
  
 - Done! You are ready to use all the functionality of the Data Providers in your services.
  * there are descriptions on the methods of what they do
   
   * The Ids are created automatically on Insert and they get returned.
   * The cascade on Foreign keys is automatic and it depends on the Nullability of the column
   
 I will soon upload here a more detailed guide of all the functionalities of this ORM, such as Stored Views and Joins
 and automatic mapping on DTOs or Updating Specific Columns.
