using BlackHole.Entities;

namespace BlackHole.Services
{
    public class BHDataInitializer
    {
        internal List<InitialCommandsAndParameters> commandsAndParameters { get; set; } = new List<InitialCommandsAndParameters>();

        public void InsertLine(string commandText)
        {
            commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = commandText , comandParameters = null });
        }

        public void InsertLine(string commandText, object valuesClass)
        {
            commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = commandText, comandParameters = valuesClass });
        }

        public void InsertLine<T>(string commandText, T entity) where T: BlackHoleEntity
        {
            commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = commandText, comandParameters = entity });
        }

        public void InsertFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string commandText = File.ReadAllText(filePath);
                commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = commandText, comandParameters = null });
            }
        }
    }

    internal class InitialCommandsAndParameters
    {
        internal string commandText { get; set; } = string.Empty;
        internal object? comandParameters { get; set; }
    }
}
