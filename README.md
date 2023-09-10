<h2>A very Fast, Fully automated and easy to setup ORM that has ALL the required features:</h2>

- Supports out of the box: SQL Server, MySql, Postgres, Oracle and SqLite.
- Handling all the databases with the same methods.
- Auto Create and Update Database on Startup.
- Tables are based on the Entities.
- Supports Autoincrement, Composite Primary Keys.
- Has Value Generator Interface , to auto generate values on Insert.
- Uses Property Attributes to declare, Foreign Keys , Default Values and more.
- Direct Mapping Entities to DTO.
- Methods for performing Joins on any table.
- Interface for Default Data.
- Using [BlackHole.Core.Cli](https://www.nuget.org/packages/BlackHole.Core.Cli) it can Parse Any Database and Generate the Entities in the app.

Find documentation here => [BlackHole Documentation](https://mikarsoft.com/BHDocumentation/index.html)
Find Example Project here => [BlackHole Example](https://github.com/Mikarsoft/BlackHole-Example-Project)
Find YouTube Tutorials here => [Mikarsoft YouTube](https://www.youtube.com/channel/UCSTW9V4wuY-nmLg0CRgL37w)


[Latest Version 6.1.0](https://www.nuget.org/packages/BlackHole.Core.ORM)

Changes:
- Maximum Performance and Memory Optimization
- Added fully customizable BHOpenEntity
- New Additional DataProvider for BHOpenEntity
- Complete user Control of  BHOpenEntity's properties
- Support for Composite PrimaryKeys in BHOpenEntities, with or without autoincrement
- Value Generator Interface that Autogenerates a column's value on insert
- Support For No PrimaryKey Table
- New DefaultValue Attribute for the Columns
- Added Option on Foreign Key Attribute to point to specific column
- Reading Automatically the DateFormat of the Database.
- Improved faster Logging.
- Upgraded Database Parsing. It can Parse any Database using the new BHOpenEntities
- Added Initialization method that doesn't require Host. For Console and Desktop Apps
- Added BlockAutoUpdate Option, to prevent automatic update on startup
- Added Option for using DoubleQuotes on SqLite and SqlServer Naming
- BlackHoleEntities and BHOpenEntities can work together on Joins methods
- Tracking and updating BHOpenEntity's values on the Insert methods

Quick Start:

- In Your project install Black.Hole.ORM from Nuget.org

- In your Program.cs add (Namespace => using BlackHole.Configuration)
  Add the following line into your IServiceCollection =>
    services.SuperNova(settings => settings.AddDatabase(connection => connection.UseSqlServer(connectionString)))

- Create some Entities in any folder that Inherit from the class 'BlackHoleEntity<int>' for Entities that are using Integer as Id,
  or 'BlackHoleEntity<Guid>' for Entities that are using Guid as Id
  or 'BlackHoleEntity<string>' for Entities that are using SHA1 hash as Id. (Namespace => using BlackHole.Entities)
  
- There is also the 'BHOpenEntity<Entity type>' Interface which is a more flexible and advanced Entity, with more capabilities, but slightly less performance.
  It is suggested to advanced Developers that want to have full control of the Entity, using Composite Keys , Autogenerated values and more..

- Add properties on your Entities except the Id property that already exists in the BlackHoleEntity class.

- Add Attributes to the properties of your Entities' 
  '[ForeignKey(typeof(Entity), nullability)]' , '[NotNullable]', [DefaultValue(object)] and '[VarCharSize(int)]'
  * You can also use '[UseActivator]' attribute on your Entity, to take advantage of the 'IsActive' column in case you need to keep the
  data after delete. (Namespace => using BlackHole.Entities)

- Make your services Inherit from 'BlackHoleScoped' or 'BlackHoleSingleton' or 'BlackHoleTransient' so they will be automatically
  registered on the Startup to the IServiceCollection. (Namespace => using BlackHole.Services)
  
 - Last step , go to your services or your controllers and add the Interfaces for the DataProviders =>
  private readonly IBHDataProvider<Entity,IdType> _entityService;
  private readonly IBHOpenDataProvider<BHOpenEntity> _openEntityService;
  Example: IBHDataProvider<Customer,Guid> _customerService; (Namespace => using BlackHole.Core)
  
 - Done! You are ready to use all the functionality of the Data Providers in your services.
 - Data Providers contain all the required methods to cimmunicate with the database.

Visit [Mikarsoft Official Webpage](https://mikarsoft.com/) for more Information.
