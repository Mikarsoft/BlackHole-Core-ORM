using BlackHole.CoreSupport;

namespace BlackHole.Internal
{
    internal class BHParsingColumnScanner
    {
        private readonly IExecutionProvider _connection;

        internal BHParsingColumnScanner(IExecutionProvider connection)
        {
            _connection = connection;
        }

        internal ColumnScanResult ParseColumnToProperty(TableParsingInfo tableColumnInfo)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }

        internal ColumnScanResult ParsePrimaryKeyToProperty(TableParsingInfo tableColumnInfo)
        {
            ColumnScanResult scanResult = new ColumnScanResult();
            return scanResult;
        }
    }
}
