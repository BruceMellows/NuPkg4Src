// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: struct MIB_TCP6TABLE_OWNER_PID
// NuPkg4Src-Tags: CSharp Source MIB TCP6TABLE OWNER PID iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.dbghelp.MIB_TCP6TABLE_OWNER_PID
// NuPkg4Src-ExternalSourceDependencies: PInvoke.dbghelp.MIB_TCP6ROW_OWNER_PID
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-Hash: SHA512Managed:9AADC64AB2F8205FDE9DF25FAF6EE8656E297759D0368077584786F364CD4027E8E453D2A51FE9B881C14A542B8B4A05ED9DAF03C5CBD7D141BB348AEF21317B
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace PInvoke.iphlpapi
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <see cref="http://www.pinvoke.net/default.aspx/iphlpapi.getextendedtcptable"/>
    /// </summary>
    internal static partial class NativeMethods
    {
        // https://msdn.microsoft.com/en-us/library/windows/desktop/aa366905
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCP6TABLE_OWNER_PID
        {
            public uint dwNumEntries;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
            public MIB_TCP6ROW_OWNER_PID[] table;
        }
    }
}
