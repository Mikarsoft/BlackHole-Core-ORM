using Microsoft.Extensions.DependencyInjection;

namespace Mikarsoft.BlackHoleCore.Configuration
{
    public static class BlackHoleConfiguration
    {
        public static IServiceCollection SupaNova(this IServiceCollection sv, Action<BHBaseConfig> config)
        {
            BHBaseConfig configuration = new BHBaseConfig();
            config.Invoke(configuration);


            return sv;
        }
    }
}
