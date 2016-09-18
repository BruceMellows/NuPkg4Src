// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: struct TCP_TABLE_CLASS
// NuPkg4Src-Tags: CSharp Source TCP TABLE CLASS iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.iphlpapi.TCP_TABLE_CLASS
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-Hash: SHA512Managed:0CEA864E17AD04B3DB3DF388C0F28B97E38E79BEAB26C7172EC87C158BAD2587F4F9E8F2B65FB65962CF827C376FE0C5B36D64CFC0DB39C2E2BC5FCC697A01F0
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
        // https://msdn2.microsoft.com/en-us/library/aa366386.aspx
        public enum TCP_TABLE_CLASS
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }
    }
}
