using BlackHole.Ray;

namespace BlackHole.RayCore
{
    internal class RayCMDWrapper
    {
        private RayStatementHandle _stmt;                  // hStmt
        private Ray.RayStatementHandle _keyinfostmt;           // hStmt for keyinfo

        internal RayDescriptorHandle _hdesc;              // hDesc

        internal CNativeBuffer _nativeParameterBuffer;      // Native memory for internal memory management
        // (Performance optimization)

        internal CNativeBuffer _dataReaderBuf;         // Reusable DataReader buffer

        private readonly RayConnection _connection;        // Connection
        private bool _canceling;             // true if the command is canceling
        internal bool _hasBoundColumns;
        internal bool _ssKeyInfoModeOn;       // tells us if the SqlServer specific options are on
        internal bool _ssKeyInfoModeOff;      // a tri-state value would be much better ...

        internal RayCMDWrapper(RayConnection connection)
        {
            _connection = connection;
        }

        internal RayConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        internal RayStatementHandle StatementHandle
        {
            get { return _stmt; }
        }
    }
}
