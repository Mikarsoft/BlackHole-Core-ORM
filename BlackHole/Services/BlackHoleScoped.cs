namespace BlackHole.Services
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Scoped
    /// <para><b>Important</b> => If this service has also an Interface. The Name of the Interface, minus the first letter, must be
    /// contained into the Name of the Service class in order to be found.</para>
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
        /// <para><b>Important</b> => If this service has also an Interface. The Name of the Interface, minus the first letter, must be
        /// contained into the Name of the Service class in order to be found.</para>
        /// </summary>
        public BlackHoleScoped()
        {
            Type type = GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
