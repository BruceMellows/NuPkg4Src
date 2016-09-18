// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: struct MIB_TCP6ROW_OWNER_PID
// NuPkg4Src-Tags: CSharp Source MIB TCP6ROW OWNER PID iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.iphlpapi.MIB_TCP6ROW_OWNER_PID
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-Hash: SHA512Managed:94638611CDFFD606B50D9D63AA5C0B554A4839DDCE84C338A690D3508C4FBF8E35041487E141D99DAECC0CFBB2DADD20D0B8F16F5D7F002065EA079933E7F127
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
        // https://msdn.microsoft.com/en-us/library/aa366896
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCP6ROW_OWNER_PID
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] localAddr;

            public uint localScopeId;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] remoteAddr;

            public uint remoteScopeId;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] remotePort;

            public uint state;
            public uint owningPid;
        }
    }
}
