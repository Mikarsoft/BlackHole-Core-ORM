using BlackHole.Engine;

namespace BlackHole.Core
{
    /// <summary>
    /// An Object that can store Dynamic Parameters that get
    /// translated into SQL Parameters in every provider.
    /// </summary>
    public class BHParameters
    {
        internal List<BlackHoleParameter> Parameters = new();

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
