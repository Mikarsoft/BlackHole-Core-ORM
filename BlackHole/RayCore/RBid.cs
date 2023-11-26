
using BlackHole.Ray;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace BlackHole.RayCore
{
    internal static class RBid
    {
        private static ApiGroup modFlags;
        private const string dllName = "System.Data.dll";

        internal enum ApiGroup : uint
        {
            Off = 0x00000000,

            Default = 0x00000001,   // Bid.TraceEx (Always ON)
            Trace = 0x00000002,   // Bid.Trace, Bid.PutStr
            Scope = 0x00000004,   // Bid.Scope{Enter|Leave|Auto}
            Perf = 0x00000008,   // TBD..
            Resource = 0x00000010,   // TBD..
            Memory = 0x00000020,   // TBD..
            StatusOk = 0x00000040,   // S_OK, STATUS_SUCCESS, etc.
            Advanced = 0x00000080,   // Bid.TraceEx

            Pooling = 0x00001000,
            Dependency = 0x00002000,
            StateDump = 0x00004000,
            Correlation = 0x00040000,

            MaskBid = 0x00000FFF,
            MaskUser = 0xFFFFF000,
            MaskAll = 0xFFFFFFFF
        }

        internal static bool TraceOn
        {
            [BidMethod(Enabled = false)] // Ignore this method in FXCopBid rule
            get { return (modFlags & ApiGroup.Trace) != 0; }
        }

        internal static bool ScopeOn
        {
            get { return (modFlags & ApiGroup.Scope) != 0; }
        }

        internal static bool AdvancedOn
        {
            get { return (modFlags & ApiGroup.Advanced) != 0; }
        }

        internal static bool IsOn(ApiGroup flag)
        {
            return (modFlags & flag) != 0;
        }

        private static IntPtr __noData;

        internal static IntPtr NoData
        {
            get { return __noData; }
        }

        internal static IntPtr ID
        {
            get { return modID; }
        }

        internal static bool IsInitialized
        {
            get { return modID != NoData; }
        }

        internal struct ModeFlags
        {
            internal const uint
                Default = 0x00,
                SmartNewLine = 0x01,
                NewLine = 0x02,

                Enabled = 0x04,
                /*DemandSrc   = 0x08,*/

                Blob = 0x10,
                BlobCopy = 0x12,
                BlobBinMode = 0x14;
        }

        internal static void PutStr(string str)
        {
            if ((modFlags & ApiGroup.Trace) != 0 && modID != NoData)
                NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.Default, str);
        }

        [BidMethod]
        internal static void Trace(string strConst)
        {
            if ((modFlags & ApiGroup.Trace) != 0 && modID != NoData)
                NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, strConst);
        }

        [BidMethod]
        internal static void TraceEx(uint flags, string strConst)
        {
            if (modID != NoData)
                NativeMethods.Trace(modID, UIntPtr.Zero, (UIntPtr)flags, strConst);
        }

        [BidMethod]
        internal static void Trace(string fmtPrintfW, string a1)
        {
            if ((modFlags & ApiGroup.Trace) != 0 && modID != NoData)
                NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
        }

        [BidMethod]
        internal static void TraceEx(uint flags, string fmtPrintfW, string a1)
        {
            if (modID != NoData)
                NativeMethods.Trace(modID, UIntPtr.Zero, (UIntPtr)flags, fmtPrintfW, a1);
        }

        internal static void ScopeLeave(ref IntPtr hScp)
        {
            if ((modFlags & ApiGroup.Scope) != 0 && modID != NoData)
            {
                if (hScp != NoData) NativeMethods.ScopeLeave(modID, UIntPtr.Zero, UIntPtr.Zero, ref hScp);
            }
            else
            {
                hScp = NoData;  // NOTE: This assignment is necessary, even it may look useless
            }
        }

        [BidMethod]
        internal static void ScopeEnter(out IntPtr hScp, string strConst)
        {
            if ((modFlags & ApiGroup.Scope) != 0 && modID != NoData)
            {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, strConst);
            }
            else
            {
                hScp = NoData;
            }
        }

        [BidMethod]
        internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1)
        {
            if ((modFlags & ApiGroup.Scope) != 0 && modID != NoData)
            {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
            }
            else
            {
                hScp = NoData;
            }
        }

        [BidMethod]
        internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, int a1, int a2)
        {
            if ((modFlags & ApiGroup.Scope) != 0 && modID != NoData)
            {
                NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
            }
            else
            {
                hScp = NoData;
            }
        }

        [BidMethod(Enabled = false)]
        internal static void TraceBin(string constStrHeader, byte[] buff, UInt16 length)
        {
            if (modID != NoData)
            {
                if (constStrHeader != null && constStrHeader.Length > 0)
                {
                    NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine, constStrHeader);
                }
                if ((UInt16)buff.Length < length)
                {
                    length = (UInt16)buff.Length;
                }
                NativeMethods.TraceBin(modID, UIntPtr.Zero, (UIntPtr)Bid.ModeFlags.Blob,
                                        "<Trace|BLOB> %p %u\n", buff, length);
            }
        }

        [BidMethod(Enabled = false)] // do not validate calls to this method in FXCopBid
        internal static void TraceBinEx(byte[] buff, UInt16 length)
        {
            if (modID != NoData)
            {
                if ((UInt16)buff.Length < length)
                {
                    length = (UInt16)buff.Length;
                }
                NativeMethods.TraceBin(modID, UIntPtr.Zero, (UIntPtr)Bid.ModeFlags.Blob,
                                        "<Trace|BLOB> %p %u\n", buff, length);
            }
        }

        internal static ApiGroup SetApiGroupBits(ApiGroup mask, ApiGroup bits)
        {
            lock (_setBitsLock)
            {
                ApiGroup tmp = modFlags;
                if (mask != ApiGroup.Off)
                {
                    modFlags ^= (bits ^ tmp) & mask;
                }
                return tmp;
            }
        }
        private static object _setBitsLock = new object();

        internal static bool AddMetaText(string metaStr)
        {
            if (modID != NoData)
            {
                NativeMethods.AddMetaText(modID, DefaultCmdSpace, CtlCmd.AddMetaText, IntPtr.Zero, metaStr, IntPtr.Zero);
            }
            return true;
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal static void DTRACE(string strConst)
        {
            if ((modFlags & ApiGroup.Trace) != 0 && modID != NoData)
            {
                NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine, strConst);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal static void DTRACE(string clrFormatString, params object[] args)
        {
            if ((modFlags & ApiGroup.Trace) != 0 && modID != NoData)
            {
                NativeMethods.PutStr(modID, UIntPtr.Zero, (UIntPtr)ModeFlags.SmartNewLine,
                                    String.Format(CultureInfo.CurrentCulture, clrFormatString, args));
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        internal static void DASSERT(bool condition)
        {
            if (!condition)
            {
                System.Diagnostics.Trace.Assert(false);
            }
        }

        private
        static IntPtr modID = internalInitialize();

        private static string? modIdentity;

        private delegate ApiGroup CtrlCB(ApiGroup mask, ApiGroup bits);
        private static CtrlCB? ctrlCallback;

        [StructLayout(LayoutKind.Sequential)]
        private class BindingCookie
        {
            internal IntPtr _data;
            internal BindingCookie() { _data = (IntPtr)(-1); }

            internal void Invalidate() { _data = (IntPtr)(-1); }
        };

        private static BindingCookie cookieObject = new();
        private static GCHandle hCookie;

        private static void deterministicStaticInit()
        {
            __noData = (IntPtr)(-1);
            __defaultCmdSpace = (IntPtr)(-1);

            modFlags = ApiGroup.Off;
            modIdentity = string.Empty;
            ctrlCallback = new CtrlCB(SetApiGroupBits);

            cookieObject = new BindingCookie();
            hCookie = GCHandle.Alloc(cookieObject, GCHandleType.Pinned);
        }

        private static IntPtr __defaultCmdSpace;

        internal static IntPtr DefaultCmdSpace
        {
            get { return __defaultCmdSpace; }
        }

        private
        enum CtlCmd : uint
        {
            //
            //  Standard modifiers for command codes.
            //
            Reverse = 1,
            Unicode = 2,

            //
            //  Predefined commands are in range [CtlCmd.DcsBase .. CtlCmd.DcsMax]
            //  'Dcs' stands for 'Default Command Space'
            //
            DcsBase = 268435456 * 4,    // 0x10000000 * 4
            DcsMax = 402653183 * 4,    // 0x17FFFFFF * 4

            //
            //  Control Panel commands are in range [CtlCmd.CplBase .. CtlCmd.CplMax]
            //
            CplBase = 402653184 * 4,    // 0x18000000 * 4
            CplMax = 536870911 * 4,    // 0x1FFFFFFF * 4

            //
            //  Predefined commands (have wrapper functions)
            //
            CmdSpaceCount = 0 * 4 + DcsBase,
            CmdSpaceEnum = 1 * 4 + DcsBase,
            CmdSpaceQuery = 2 * 4 + DcsBase,

            GetEventID = 5 * 4 + DcsBase + Unicode,
            ParseString = 6 * 4 + DcsBase + Unicode,
            AddExtension = 7 * 4 + DcsBase + Unicode,
            AddMetaText = 8 * 4 + DcsBase + Unicode,
            AddResHandle = 9 * 4 + DcsBase + Unicode,
            Shutdown = 10 * 4 + DcsBase + Unicode,

            LastItem

        } // CtlCmd

        private const int BidVer = 9210;

        [StructLayout(LayoutKind.Sequential)]
        private struct BIDEXTINFO
        {
            IntPtr hModule;
            [MarshalAs(UnmanagedType.LPWStr)]
            string DomainName;
            int Reserved2;
            int Reserved;
            [MarshalAs(UnmanagedType.LPWStr)]
            string ModulePath;
            IntPtr ModulePathA;
            IntPtr pBindCookie;

            internal BIDEXTINFO(IntPtr hMod, string modPath, string friendlyName, IntPtr cookiePtr)
            {
                hModule = hMod;
                DomainName = friendlyName;
                Reserved2 = 0;
                Reserved = 0;
                ModulePath = modPath;
                ModulePathA = IntPtr.Zero;
                pBindCookie = cookiePtr;
            }
        }; // BIDEXTINFO

        private static string getIdentity(Module mod)
        {
            string idStr;
            object[] attrColl = mod.GetCustomAttributes(typeof(BidIdentityAttribute), true);
            if (attrColl.Length == 0)
            {
                idStr = mod.Name;
            }
            else
            {
                idStr = ((BidIdentityAttribute)attrColl[0]).IdentityString;
            }
            return idStr;
        }

        private static string getAppDomainFriendlyName()
        {
            string name = AppDomain.CurrentDomain.FriendlyName;
            if (name == null || name.Length <= 0)
            {
                name = "AppDomain.H" + AppDomain.CurrentDomain.GetHashCode();
            }

            return VersioningHelper.MakeVersionSafeName(name, ResourceScope.Machine, ResourceScope.AppDomain);
        }

        private const uint configFlags = 0xD0000000; // ACTIVE_BID|CTLCALLBACK|MASK_PAGE

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)] // Module.FullyQualifiedName
        private static string getModulePath(Module mod)
        {
            return mod.FullyQualifiedName;
        }

        [ResourceExposure(ResourceScope.None)] // info contained within call to DllBidEntryPoint
        [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)] // getModulePath
        private static void initEntryPoint()
        {
            NativeMethods.DllBidInitialize();

            //
            //  Multi-file assemblies are not supported by current model of the BID managed wrapper.
            //  The below Marshal.GetHINSTANCE(mod) will return HINSTANCE for the manifest module
            //  instead of actual module, which is Ok because it is the only module
            //  in the single-file assembly.
            //
            Module mod = Assembly.GetExecutingAssembly().ManifestModule;
            modIdentity = getIdentity(mod);
            modID = NoData;

            string friendlyName = getAppDomainFriendlyName();
            BIDEXTINFO extInfo = new BIDEXTINFO(Marshal.GetHINSTANCE(mod),
                                                getModulePath(mod),
                                                friendlyName,
                                                hCookie.AddrOfPinnedObject());

            NativeMethods.DllBidEntryPoint(ref modID, BidVer, modIdentity,
                                            configFlags, ref modFlags, ctrlCallback ?? new(SetApiGroupBits),
                                            ref extInfo, IntPtr.Zero, IntPtr.Zero);

            if (modID != NoData)
            {
                object[] attrColl = mod.GetCustomAttributes(typeof(BidMetaTextAttribute), true);
                foreach (object obj in attrColl)
                {
                    AddMetaText(((BidMetaTextAttribute)obj).MetaText);
                }

                Bid.Trace("<ds.Bid|Info> VersionSafeName='%ls'\n", friendlyName);
            }
        } // initEntryPoint

        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
        private static void doneEntryPoint()
        {
            if (modID == NoData)
            {
                modFlags = ApiGroup.Off;
                return;
            }

            try
            {
                NativeMethods.DllBidEntryPoint(ref modID, 0, IntPtr.Zero,
                                                configFlags, ref modFlags,
                                                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                NativeMethods.DllBidFinalize();
            }
            catch
            {
                //
                //  We do intentionally catch everything because no matter what happens
                //  we don't want any exception to escape when we're in context of a finalizer.
                //  Note that critical exceptions such as ThreadAbortException could be
                //  propagated anyway (CLR 2.0 and above).
                //
                modFlags = ApiGroup.Off;    // This is 'NoOp', just to not have empty catch block.
            }
            finally
            {
                cookieObject.Invalidate();
                modID = NoData;
                modFlags = ApiGroup.Off;
            }

        } // doneEntryPoint

        private sealed class AutoInit : SafeHandle
        {
            internal AutoInit() : base(IntPtr.Zero, true)
            {
                initEntryPoint();
                _bInitialized = true;
            }
            override protected bool ReleaseHandle()
            {
                _bInitialized = false;
                doneEntryPoint();
                return true;
            }
            public override bool IsInvalid
            {
                get { return !_bInitialized; }
            }
            private bool _bInitialized;
        }

        private static AutoInit ai = new();

        private static IntPtr internalInitialize()
        {
            deterministicStaticInit();
            ai = new AutoInit();
            return modID;
        }

        [SuppressUnmanagedCodeSecurity, ComVisible(false)]
        private static partial class NativeMethods
        {
            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
            EntryPoint = "DllBidPutStrW")]
            extern
            internal static void PutStr(IntPtr hID, UIntPtr src, UIntPtr info, string str);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DllBidTraceCW")]
            extern
            internal static void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string strConst);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DllBidTraceCW")]
            extern
            internal static void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string a1);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, EntryPoint = "DllBidScopeLeave")]
            extern
            internal static void ScopeLeave(IntPtr hID, UIntPtr src, UIntPtr info, ref IntPtr hScp);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DllBidScopeEnterCW")]
            extern
            internal static void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string strConst);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DllBidScopeEnterCW")]
            extern
            internal static void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp,
                                             string fmtPrintfW, int a1);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DllBidScopeEnterCW")]
            extern
            internal static void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp,
                                             string fmtPrintfW, int a1, int a2);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DllBidTraceCW")]
            extern
            internal static void TraceBin(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, byte[] buff, UInt32 len);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName, CharSet = CharSet.Unicode, EntryPoint = "DllBidCtlProc")]
            extern
            internal static void AddMetaText(IntPtr hID, IntPtr cmdSpace, CtlCmd cmd, IntPtr nop1,
                                  string txtID, IntPtr nop2);

            [ResourceExposure(ResourceScope.Machine)]
            [DllImport(dllName, CharSet = CharSet.Ansi, BestFitMapping = false)]
            extern
            internal static void DllBidEntryPoint(ref IntPtr hID, int bInitAndVer, string sIdentity,
                                                uint propBits, ref ApiGroup pGblFlags, CtrlCB fAddr,
                                                ref BIDEXTINFO pExtInfo, IntPtr pHooks, IntPtr pHdr);

            [ResourceExposure(ResourceScope.Machine)]
            [DllImport(dllName)]
            extern
            internal static void DllBidEntryPoint(ref IntPtr hID, int bInitAndVer, IntPtr unused1,
                                                uint propBits, ref ApiGroup pGblFlags, IntPtr unused2,
                                                IntPtr unused3, IntPtr unused4, IntPtr unused5);

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName)]
            extern
            internal static void DllBidInitialize();

            [ResourceExposure(ResourceScope.None)]
            [DllImport(dllName)]
            extern
            internal static void DllBidFinalize();
        }
    }

    [AttributeUsage(AttributeTargets.Module, AllowMultiple = false)]
    internal sealed class BidIdentityAttribute : Attribute
    {
        internal BidIdentityAttribute(string idStr)
        {
            _identity = idStr;
        }
        internal string IdentityString
        {
            get { return _identity; }
        }
        string _identity;
    }

    [System.Diagnostics.Conditional("CODE_ANALYSIS")]
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class BidMethodAttribute : Attribute
    {
        private bool m_enabled;

        /// <summary>
        /// enabled by default
        /// </summary>
        internal BidMethodAttribute()
        {
            m_enabled = true;
        }

        /// <summary>
        /// if Enabled is true, FxCopBid rule will validate all calls to this method and require that it will have string argument;
        /// otherwise, this method is ignored.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }
    }
}
