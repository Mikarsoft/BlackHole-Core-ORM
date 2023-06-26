# BlackHole-Core-ORM
A fully automated, very easy to use and setup, with many new features, Object Relational Mapping Library, for .Net Core 6 and 7. Using Custom Data Providers that are 3x times faster than EF Core and Dapper. Many interfaces for easy Reading and Writing Data. Extention Methods for Joining any tables.
It Supports SQL SERVER, MYSQL, POSTGRESQL, ORACLE and SQLITE.

6.0.1  is released.

  Changes:
    - Added Schema support for SqlServer and Postgres
    
    - Added support for the new [BlackHole-Core-Cli](https://github.com/Mikarsoft/BlackHole-Core-Cli) , that adds the 'Database First' capability and more.
    
    - Added support for some famous sql funtions in the 'Where' statement.
    
    - Added support for other custom methods and popular dotnet methods in the 'Where' statement. Like => string.Contains(), string.Replace() etc.
    
    - Added Initial Data support using the interface 'IBHInitialData' you can run insert commands on the creation of your database and store some default data.
      It can also load sql commands from files.
      
    - Addes support for Nullable properties in the 'BlackHoleEntity'.
    
    - Added Timeout Setting for the sql commands in the configuration's options.
    
    - Improved speed and reliability of the ExpressionToSql translator.
    
    - Fixed outer joins bug.


Example:

 -Find an Example Project here => [BlackHole Example](https://github.com/Mikarsoft/BlackHole-Example-Project)
 
Documentation:

 -Find Online Documentation here => [BlackHole Documentation](https://mikarsoft.com/BHDocumentation/index.html)

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
