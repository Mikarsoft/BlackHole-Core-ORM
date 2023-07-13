namespace BlackHole.Services
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Transient
    /// </summary>
    public abstract partial class BlackHoleTransient
    {
        /// <summary>
        /// The class of the Service
        /// </summary>
        public Type ServiceType { get; set; }
        /// <summary>
        /// The interface of the Service
        /// </summary>
        public Type? InterfaceType { get; set; }

        /// <summary>
        /// Make a service Inherit from this class
        /// to automatically get registered as Transient
        /// </summary>
        public BlackHoleTransient()
        {
            Type type = GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
