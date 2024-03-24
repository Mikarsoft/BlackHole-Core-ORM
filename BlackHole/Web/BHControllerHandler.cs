using Microsoft.Extensions.DependencyInjection;

namespace BlackHole.Web
{
    internal class BHControllerHandler
    {
        public T CreateControllers<T>(IServiceProvider serviceProvider, T controllerType)
        {
            //IServiceProvider serviceProvider = services.BuildServiceProvider();
            //Type controllerType = GetControllerType(); // Determine the controller type
            //object controllerInstance = serviceProvider.GetRequiredService(controllerType);

            return (T)serviceProvider.GetRequiredService(typeof(T));
        }
    }
}
