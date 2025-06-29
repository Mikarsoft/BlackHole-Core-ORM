

using Mikarsoft.BlackHoleCore.Entities;

namespace Mikarsoft.BlackHoleCore
{
    public interface IMMethods<G>
    {
        G First { get; }

        G Last { get; }


        BHResult<byte> Max(Func<G, byte> selector);

        BHResult<int> Max(Func<G, int?> selector);

        BHResult<double> Max(Func<G, double?> selector);

        BHResult<decimal> Max(Func<G, decimal?> selector);

        BHResult<long> Max(Func<G, long?> selector);

        BHResult<short> Max(Func<G, short?> selector);

        BHResult<DateTime> Max(Func<G, DateTime?> selector);

        BHResult<DateTimeOffset> Max(Func<G, DateTimeOffset?> selector);


        BHResult<byte> Min(Func<G, byte> selector);

        BHResult<int> Min(Func<G, int?> selector);

        BHResult<double> Min(Func<G, double?> selector);

        BHResult<decimal> Min(Func<G, decimal?> selector);

        BHResult<long> Min(Func<G, long?> selector);

        BHResult<short> Min(Func<G, short?> selector);

        BHResult<DateTime> Min(Func<G, DateTime?> selector);

        BHResult<DateTimeOffset> Min(Func<G, DateTimeOffset?> selector);


        BHResult<decimal> Average(Func<G, byte> selector);

        BHResult<decimal> Average(Func<G, int?> selector);

        BHResult<decimal> Average(Func<G, double?> selector);

        BHResult<decimal> Average(Func<G, decimal?> selector);

        BHResult<decimal> Average(Func<G, long?> selector);

        BHResult<decimal> Average(Func<G, short?> selector);
    }
}
