using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackHole.Ray
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    internal sealed class RayConnectionString : DbConnectionOptions
    {
        // instances of this class are intended to be immutable, i.e readonly
        // used by pooling classes so it is much easier to verify correctness
        // when not worried about the class being modified during execution

        private static class KEY
        {
            internal const string SaveFile = "savefile";
        }

        private readonly string _expandedConnectionString;

        internal RayConnectionString(string connectionString, bool validate) : base(connectionString, null, true)
        {
            if (!validate)
            {
                string filename = null;
                int position = 0;
                _expandedConnectionString = ExpandDataDirectories(ref filename, ref position);
            }
            if (validate || (null == _expandedConnectionString))
            {
                // do not check string length if it was expanded because the final result may be shorter than the original
                if ((null != connectionString) && (Ray32.MAX_CONNECTION_STRING_LENGTH < connectionString.Length))
                { // MDAC 83536
                    throw Ray.ConnectionStringTooLong();
                }
            }
        }

        protected internal override string Expand()
        {
            if (null != _expandedConnectionString)
            {
                return _expandedConnectionString;
            }
            else
            {
                return base.Expand();
            }
        }
    }
}
