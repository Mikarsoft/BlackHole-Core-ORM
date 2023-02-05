
namespace BlackHole.Entities
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Transient
    /// </summary>
    public abstract partial class BlackHoleTransient
    {
        public Type ServiceType { get; set; }
        public Type? InterfaceType { get; set; }

        public BlackHoleTransient()
        {
            Type type = this.GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
