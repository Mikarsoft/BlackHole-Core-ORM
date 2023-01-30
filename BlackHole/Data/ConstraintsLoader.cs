using BlackHole.Entities;
using BlackHole.Enums;
using BlackHole.Interfaces;
using BlackHole.Statics;
using Dapper;
using System.Data;

namespace BlackHole.Data
{
    internal class ConstraintsLoader : IConstraintsLoader
    {
        private readonly IDbConnection _connection;
        private readonly IBHDatabaseSelector _multiDatabaseSelector;
        private readonly ILoggerService _loggerService;

        public ConstraintsLoader(IBHDatabaseSelector multiDatabaseSelector, ILoggerService loggerService)
        {
            _multiDatabaseSelector = multiDatabaseSelector;
            _connection = _multiDatabaseSelector.GetConnection();
            _loggerService = loggerService;
        }

        /// <summary>
        /// Reads the database and stores the constraints in a static Class. It is required for the Automatic Cascade system.
        /// </summary>
        void IConstraintsLoader.StoreAllConstraints()
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            try
            {
                using (IDbConnection connnection = _connection)
                {
                    switch (_multiDatabaseSelector.GetSqlType())
                    {
                        case BHSqlTypes.MsSql:
                            Constraints = LoadMsSqlConstraints(connnection);
                            break;
                        case BHSqlTypes.MySql:
                            Constraints = LoadMySqlConstraints(connnection);
                            break;
                        case BHSqlTypes.Postgres:
                            Constraints = LoadPgConstraints(connnection);
                            break;
                        case BHSqlTypes.SqlLite:
                            Constraints = LoadSqLiteConstraints(connnection);
                            break;
                    };
                }
            }
            catch(Exception ex)
            {
                _loggerService.CreateErrorLogs("ConstraintsLoader_",ex.Message,ex.ToString());
            }

            CascadeRelations.dataConstrains = Constraints;
        }

        private List<DataConstraints> LoadMySqlConstraints(IDbConnection connection)
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT K.TABLE_NAME, K.COLUMN_NAME, K.REFERENCED_TABLE_NAME, C.IS_NULLABLE FROM
                INFORMATION_SCHEMA.KEY_COLUMN_USAGE K
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON (C.COLUMN_NAME= K.COLUMN_NAME AND C.TABLE_NAME=K.TABLE_NAME)
                WHERE REFERENCED_TABLE_NAME is not null";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand).ToList();
            return Constraints;
        }

        private List<DataConstraints> LoadMsSqlConstraints(IDbConnection connection)
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT 
                K.TABLE_NAME,K.COLUMN_NAME,REFERENCED_TABLE_NAME=TC.TABLE_NAME,C.IS_NULLABLE FROM
	            INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS T
	            INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON TC.CONSTRAINT_NAME=T.UNIQUE_CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K ON K.CONSTRAINT_NAME= T.CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON (C.COLUMN_NAME= K.COLUMN_NAME AND C.TABLE_NAME=K.TABLE_NAME)";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand).ToList();
            return Constraints;
        }

        private List<DataConstraints> LoadPgConstraints(IDbConnection connection)
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            string GetConstrainsCommand = @"SELECT T.TABLE_NAME AS ""TABLE_NAME"", K.COLUMN_NAME AS ""COLUMN_NAME"",CU.TABLE_NAME AS ""REFERENCED_TABLE_NAME"",
                C.IS_NULLABLE AS ""IS_NULLABLE"" FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS T
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE K ON K.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                INNER JOIN INFORMATION_SCHEMA.COLUMNS C ON(C.COLUMN_NAME = K.COLUMN_NAME AND C.TABLE_NAME = K.TABLE_NAME)
                INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CU ON CU.CONSTRAINT_NAME = T.CONSTRAINT_NAME
                WHERE T.CONSTRAINT_TYPE = 'FOREIGN KEY'";
            Constraints = connection.Query<DataConstraints>(GetConstrainsCommand).ToList();
            return Constraints;
        }

        private List<DataConstraints> LoadSqLiteConstraints(IDbConnection connection)
        {
            List<DataConstraints> Constraints = new List<DataConstraints>();
            Constraints.Add(new DataConstraints { });
            return Constraints;
        }
    }

}
