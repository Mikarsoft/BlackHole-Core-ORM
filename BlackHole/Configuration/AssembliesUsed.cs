using System.Reflection;

namespace BlackHole.Configuration
{
    internal class AssembliesUsed
    {
        internal Assembly? ScanAssembly { get; set; }
        internal string AssName { get; set; } = string.Empty;
    }
}
