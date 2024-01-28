using BlackHole.Engine;

namespace BlackHole.Core
{
    internal class BHParameters : IBHParameters
    {
        internal List<BlackHoleParameter> Parameters { get; set; }

        internal BHParameters()
        {
            Parameters = new();
        } 

        /// <summary>
        /// Adds a parameter with name and value
        /// </summary>
        /// <param name="Name">Name of the Parameter</param>
        /// <param name="Value">Value of the Parameter</param>
        public void Add(string? Name , object? Value)
        {
            Parameters.Add(new BlackHoleParameter { Name = @Name , Value = Value });
        }

        /// <summary>
        /// Removes all the parameters from this Object
        /// </summary>
        public void Clear()
        {
            Parameters.Clear();
        }
    }
}
