using Mikarsoft.BlackHoleCore.Connector;

namespace Mikarsoft.BlackHoleCore
{
    internal static class BHServiceInjector
    {
        internal static IBHServiceProvider? _servicesProvider { get; set; }

        internal static IBHMethodParser GetMethodParser()
        {
            if (_servicesProvider == null)
            {
                throw new InvalidOperationException("Service Provider is missing");
            }

            return _servicesProvider.GetMethodParser();
        }

        internal static IBHDataProvider GetDataProvider()
        {
            if (_servicesProvider == null)
            {
                throw new InvalidOperationException("Service Provider is missing");
            }

            return _servicesProvider.GetDataProvider();
        }

        internal static IBHCommandBuilder GetCommandBuilder()
        {
            if (_servicesProvider == null)
            {
                throw new InvalidOperationException("Service Provider is missing");
            }

            return _servicesProvider.GetCommandBuilder();
        }

        internal static IBHInnerTransaction GetInnerTransaction()
        {
            if (_servicesProvider == null)
            {
                throw new InvalidOperationException("Service Provider is missing");
            }

            return _servicesProvider.GetInnerTransaction();
        }
    }
}
