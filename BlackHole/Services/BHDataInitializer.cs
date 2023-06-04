using BlackHole.Entities;

namespace BlackHole.Services
{
    public class BHDataInitializer
    {
        internal List<InitialCommandsAndParameters> commandsAndParameters { get; set; } = new List<InitialCommandsAndParameters>();

        public void ExecuteCommand(string commandText)
        {
            commandsAndParameters.Add(new InitialCommandsAndParameters { commandText = commandText});
        }

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
