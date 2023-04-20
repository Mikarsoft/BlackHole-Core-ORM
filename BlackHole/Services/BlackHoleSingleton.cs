﻿namespace BlackHole.Services
{
    /// <summary>
    /// Make a service Inherit from this class
    /// to automatically get registered as Singleton
    /// </summary>
    public abstract partial class BlackHoleSingleton
    {
        public Type ServiceType { get; set; }
        public Type? InterfaceType { get; set; }

        /// <summary>
        /// Make a service Inherit from this class
        /// to automatically get registered as Singleton
        /// </summary>
        public BlackHoleSingleton()
        {
            Type type = GetType();
            InterfaceType = type.GetInterface($"I{type.Name}");
            ServiceType = type;
        }
    }
}
