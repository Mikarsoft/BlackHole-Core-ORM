using System.Reflection;

namespace BlackHole.Configuration
{
    public class AssembliesUsed
    {
        internal Assembly? ScanAssembly { get; set; }

        public void UseOtherAssembly(Assembly otherAssembly)
        {
            ScanAssembly = otherAssembly;
        }
    }
}
