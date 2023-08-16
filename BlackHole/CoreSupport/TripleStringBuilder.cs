
using System.Text;

namespace BlackHole.CoreSupport
{
    internal class TripleStringBuilder : IDisposable
    {
        internal StringBuilder PNSb { get; }
        internal StringBuilder PPSb { get; }
        internal StringBuilder UPSb { get; }


        internal TripleStringBuilder()
        {
            PNSb = new StringBuilder();
            PPSb = new StringBuilder();
            UPSb = new StringBuilder();
        }

        public void Dispose()
        {
            PNSb.Clear();
            PPSb.Clear();
            UPSb.Clear();
        }
    }
}
