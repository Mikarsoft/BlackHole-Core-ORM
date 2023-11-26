namespace BlackHole.RayCore
{
    internal class RayStatementHandle : RayHandle
    {
        public RayStatementHandle(RayConnectionHandle connectionHandle) : base(Ray64.SQL_HANDLE.STMT, connectionHandle)
        {
        }

        internal Ray64.RetCode NumberOfResultColumns(out short columnsAffected)
        {
            Ray64.RetCode retcode = UnsafeNativeMethods.SQLNumResultCols(this, out columnsAffected);
            RS.TraceRay(3, "SQLNumResultCols", retcode);
            return retcode;
        }
    }
}
