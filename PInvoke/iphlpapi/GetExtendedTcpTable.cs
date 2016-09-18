// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: GetExtendedTcpTable from iphlpapi
// NuPkg4Src-Tags: CSharp Source GetExtendedTcpTable iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.dbghelp.GetExtendedTcpTable
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-ExternalSourceDependencies: PInvoke.dbghelp.TCP_TABLE_CLASS
// NuPkg4Src-Hash: SHA512Managed:213443DA5C0D2AE8C4F5DC05DF140A83D7CEBE517574F051DF820ED34B20826D0872DAD5AA5A020AD98E53C3395E152FE55029A7ED6B43D3470137771C71B9D5
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
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable,
            ref int dwOutBufLen,
            [MarshalAs(UnmanagedType.Bool)] bool sort,
            int ipVersion,
            TCP_TABLE_CLASS tblClass,
            uint reserved = 0);
    }
}
