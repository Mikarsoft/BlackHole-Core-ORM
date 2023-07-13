namespace BlackHole.Services
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Scoped
    /// </summary>
    public abstract partial class BlackHoleScoped
    {
        /// <summary>
        /// The class of the service
        /// </summary>
        public object ServiceType { get; set; }
        /// <summary>
        /// The Interface of the service
        /// </summary>
        public object? InterfaceType { get; set; }

        /// <summary>
        /// Make a service Inherit from this class
        /// to automatically get registered as Scoped
        /// </summary>
        public BlackHoleScoped()
        {
            Type type = GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
