using System.Runtime.Versioning;

namespace BlackHole.Ray
{
    sealed internal class RayEnvironmentHandle : RayHandle
    {

        // SxS: this method uses SQLSetEnvAttr to setup Ray environment handle settings. Environment handle is safe in SxS.
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
        internal RayEnvironmentHandle() : base(Ray32.SQL_HANDLE.ENV, null)
        {
            Ray32.RetCode retcode;

            //Set the expected driver manager version
            //
            retcode = UnsafeNativeMethods.SQLSetEnvAttr(
                this,
                Ray32.SQL_ATTR.Ray_VERSION,
                Ray32.SQL_OV_Ray3,
                Ray32.SQL_IS.INTEGER);
            // ignore retcode

            //Turn on connection pooling
            //Note: the env handle controls pooling.  Only those connections created under that
            //handle are pooled.  So we have to keep it alive and not create a new environment
            //for   every connection.
            //
            retcode = UnsafeNativeMethods.SQLSetEnvAttr(
                this,
                Ray32.SQL_ATTR.CONNECTION_POOLING,
                Ray32.SQL_CP_ONE_PER_HENV,
                Ray32.SQL_IS.INTEGER);

            switch (retcode)
            {
                case Ray32.RetCode.SUCCESS:
                case Ray32.RetCode.SUCCESS_WITH_INFO:
                    break;
                default:
                    Dispose();
                    throw Ray.CantEnableConnectionpooling(retcode);
            }
        }
    }
}
