
namespace BlackHole.Entities
{
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
