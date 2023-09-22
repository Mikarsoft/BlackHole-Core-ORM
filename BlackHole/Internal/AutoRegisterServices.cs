using BlackHole.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlackHole.Internal
{
    internal static class AutoRegisterServices
    {
        /// <summary>
        /// Register BlazarScoped, BlazarSingleton and BlazarTransient Services of the Inserted Assembly to the Application Builder.
        /// Automatically Finds and Registers the Services and the Interfaces that have BaseType one of the above. 
        /// Important!! Interfaces must have the same name with their service with an additional capital "I" at the front of the name.
        /// Example -> Service : myService , Interface : ImyService
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        internal static IServiceCollection RegisterBHServices(this IServiceCollection services, Assembly assembly)
        {

            Type blazarScoped = typeof(BlackHoleScoped);
            Type blazarSingleton = typeof(BlackHoleSingleton);
            Type blazarTransient = typeof(BlackHoleTransient);

            var ServiceTypes = assembly.GetTypes().Where(t => t.BaseType == blazarScoped || t.BaseType == blazarSingleton || t.BaseType == blazarTransient)
                .Select(s => new { Interface = s.GetInterfaces().FirstOrDefault(x=>s.Name.Contains(x.Name.Remove(0,1))), Service = s })
                .Where(x => x.Service != null);

            foreach (var type in ServiceTypes)
            {
                if (type.Service.BaseType == blazarScoped)
                {
                    if (type.Interface != null)
                    {
                        services.AddScoped(type.Interface, type.Service);
                    }
                    else
                    {
                        services.AddScoped(type.Service);
                    }
                }

                if (type.Service.BaseType == blazarSingleton)
                {
                    if (type.Interface != null)
                    {
                        services.AddSingleton(type.Interface, type.Service);
                    }
                    else
                    {
                        services.AddSingleton(type.Service);
                    }
                }

                if (type.Service.BaseType == blazarTransient)
                {
                    if (type.Interface != null)
                    {
                        services.AddTransient(type.Interface, type.Service);
                    }
                    else
                    {
                        services.AddTransient(type.Service);
                    }
                }
            }

            return services;
        }

        /// <summary>
        ///  Register BlazarScoped, BlazarSingleton and BlazarTransient Services of the Calling Assembly to the Application Builder.
        /// Registers the Services and the Interfaces from the inserted types, that have BaseType one of the above. 
        /// Important!! Interfaces must have the same name with their service with an additional capital "I" at the front of the name.
        /// Example -> Service : myService , Interface : ImyService
        /// </summary>
        /// <param name="services"></param>
        /// <param name="blazarServices"></param>
        internal static IServiceCollection RegisterBHServicesByList(this IServiceCollection services, List<Type> blazarServices)
        {
            Type blazarScoped = typeof(BlackHoleScoped);
            Type blazarSingleton = typeof(BlackHoleSingleton);
            Type blazarTransient = typeof(BlackHoleTransient);

            var ServiceTypes = blazarServices.Select(s => new { Interface = s.GetInterfaces()
                .FirstOrDefault(x => s.Name.Contains(x.Name.Remove(0, 1))), Service = s })
                .Where(x => x.Service != null);

            foreach (var type in ServiceTypes)
            {
                if (type.Service.BaseType == blazarScoped)
                {
                    if (type.Interface != null)
                    {
                        services.AddScoped(type.Interface, type.Service);
                    }
                    else
                    {
                        services.AddScoped(type.Service);
                    }
                }

                if (type.Service.BaseType == blazarSingleton)
                {
                    if (type.Interface != null)
                    {
                        services.AddSingleton(type.Interface, type.Service);
                    }
                    else
                    {
                        services.AddSingleton(type.Service);
                    }
                }

                if (type.Service.BaseType == blazarTransient)
                {
                    if (type.Interface != null)
                    {
                        services.AddTransient(type.Interface, type.Service);
                    }
                    else
                    {
                        services.AddTransient(type.Service);
                    }
                }
            }

            return services;
        }
    }
}
