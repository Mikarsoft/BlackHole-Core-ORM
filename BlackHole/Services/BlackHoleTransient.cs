namespace BlackHole.Services
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Transient
    /// <para><b>Important</b> => If this service has also an Interface. The Name of the Interface, minus the first letter, must be
    /// contained into the Name of the Service class in order to be found.</para>
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
        /// <para><b>Important</b> => If this service has also an Interface. The Name of the Interface, minus the first letter, must be
        /// contained into the Name of the Service class in order to be found.</para>
        /// </summary>
        public BlackHoleTransient()
        {
            Type type = GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
