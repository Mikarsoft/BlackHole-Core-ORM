
namespace BlackHole.Entities
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Singleton
    /// </summary>
    public abstract partial class BlackHoleSingleton
    {
        public Type ServiceType { get; set; }
        public Type? InterfaceType { get; set; }

        public BlackHoleSingleton()
        {
            Type type = this.GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
