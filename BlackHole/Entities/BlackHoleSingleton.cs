
namespace BlackHole.Entities
{
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
