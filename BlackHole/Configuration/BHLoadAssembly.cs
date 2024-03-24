using System.Reflection;

namespace BlackHole.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class BHLoadAssembly
    {
        internal Assembly? LoadedAssembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void LoadFromPath(string path)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BHLoadAssemblies
    {
        internal List<Assembly> LoadedAssemblies { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void LoadFromPath(string path)
        {

        }
    }
}
