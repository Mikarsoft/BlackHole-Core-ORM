namespace BlackHole.RayCore
{
    internal class RayStatementHandle : RayHandle
    {
        public RayStatementHandle(bool ownsHandle) : base(Ray64.SQL_HANDLE.STMT, ownsHandle)
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
