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

        public void AddParameter(string? Name , object? Value)
        {
            Parameters.Add(new BlackHoleParameter { Name = @Name , Value = Value });
        }

        public void Clear()
        {
            Parameters.Clear();
        }
    }
}
