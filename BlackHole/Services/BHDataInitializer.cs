using BlackHole.Core;
using BlackHole.Entities;

namespace BlackHole.Services
{
    /// <summary>
    /// A simple data provider that executes the commands only once, when a database is created.
    /// </summary>
    public class BHDataInitializer
    {
        /// <summary>
        /// 
        /// </summary>
        internal BHDataInitializer() { }

        internal List<InitialCommandsAndParameters> commandsAndParameters { get; set; } = new List<InitialCommandsAndParameters>();

        /// <summary>
        /// Executes an sql command only once, right after the creation of the database.
        /// <para> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para>In case you are using a specific schema, don't forget to add the schema name before the Table name</para>
        /// </summary>
        /// <param name="commandText">Sql command including values</param>
        public void ExecuteCommand(string commandText)
        {
            commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = commandText});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="G"></typeparam>
        /// <returns></returns>
        public IBHDataProvider<T,G> GetProvider<T,G>() where T : BlackHoleEntity<G> where G:IComparable<G>
        {
            return new BHDataProvider<T, G>();
        }

        /// <summary>
        /// Reads an sql file with semi-column seperated commands and executes the commands
        /// once, right after the creation of the database.
        /// <para> For Oracle and Postgres , Double Quotes are required for the Table and Column Names in your command text</para>
        /// <para>In case you are using a specific schema, don't forget to add the schema name before the Table name</para>
        /// </summary>
        /// <param name="filePath">Full path of the sql file</param>
        public void CommandsFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string commandText = File.ReadAllText(filePath);
                string[] commandsArray = commandText.Split(";");

                foreach(string command in commandsArray)
                {
                    commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = command});
                }
            }
        }
    }

    internal class InitialCommandsAndParameters
    {
        internal string commandText { get; set; } = string.Empty;
    }
}
