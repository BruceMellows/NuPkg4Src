// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: struct MIB_TCPTABLE_OWNER_PID
// NuPkg4Src-Tags: CSharp Source MIB TCPTABLE OWNER PID iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.iphlpapi.MIB_TCPTABLE_OWNER_PID
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-ExternalSourceDependencies: PInvoke.iphlpapi.MIB_TCPROW_OWNER_PID
// NuPkg4Src-Hash: SHA512Managed:CBB98CBF3B03FCFE99F6B294DA13C24DC8E88AE9A69E308926EAB49F60404E996FE8C8BA9007F30375C3FD37455C23DA15E74E3F7963DDCF3DCDD96CAC7728F7
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
        // https://msdn2.microsoft.com/en-us/library/aa366921.aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
            public MIB_TCPROW_OWNER_PID[] table;
        }
    }
}
