using System.Runtime.InteropServices;

namespace BlackHole.RayCore
{
    sealed internal class RayDescriptorHandle : RayHandle
    {

        internal RayDescriptorHandle(RayStatementHandle statementHandle, Ray64.SQL_ATTR attribute) : base(attribute, statementHandle)
        {
        }

        internal Ray64.RetCode GetDescriptionField(int i, Ray64.SQL_DESC attribute, RayNativeBuffer buffer, out int numericAttribute)
        {
            Ray64.RetCode retcode = UnsafeNativeMethods.SQLGetDescFieldW(this, checked((short)i), attribute, buffer, buffer.ShortLength, out numericAttribute);
            RS.TraceRay(3, "SQLGetDescFieldW", retcode);
            return retcode;
        }

        internal Ray64.RetCode SetDescriptionField1(short ordinal, Ray64.SQL_DESC type, IntPtr value)
        {
            Ray64.RetCode retcode = UnsafeNativeMethods.SQLSetDescFieldW(this, ordinal, type, value, 0);
            RS.TraceRay(3, "SQLSetDescFieldW", retcode);
            return retcode;
        }

        internal Ray64.RetCode SetDescriptionField2(short ordinal, Ray64.SQL_DESC type, HandleRef value)
        {
            Ray64.RetCode retcode = UnsafeNativeMethods.SQLSetDescFieldW(this, ordinal, type, value, 0);
            RS.TraceRay(3, "SQLSetDescFieldW", retcode);
            return retcode;
        }
    }
}
