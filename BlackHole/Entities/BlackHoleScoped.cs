
namespace BlackHole.Entities
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Scoped
    /// </summary>
    public abstract partial class BlackHoleScoped
    {
        public object ServiceType { get; set; }
        public object? InterfaceType { get; set; }

        public BlackHoleScoped()
        {
            Type type = this.GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
