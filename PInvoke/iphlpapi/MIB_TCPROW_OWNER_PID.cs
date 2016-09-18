// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: struct MIB_TCPROW_OWNER_PID
// NuPkg4Src-Tags: CSharp Source MIB TCPROW OWNER PID iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.iphlpapi.MIB_TCPROW_OWNER_PID
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-Hash: SHA512Managed:CFA78CC2EE6D84C62BF02BB67330A297DA951448A541E78A52F15F1C10E79509722925A747B9A57C8050DB2468BBC6E8F87944136FF5FEC7B24D3AD5B68B0AB9
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
        // https://msdn2.microsoft.com/en-us/library/aa366913.aspx
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            public uint state;
            public uint localAddr;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] localPort;

            public uint remoteAddr;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] remotePort;

            public uint owningPid;
        }
    }
}
