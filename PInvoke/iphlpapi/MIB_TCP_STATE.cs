// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: enum MIB_TCP_STATE
// NuPkg4Src-Tags: CSharp Source MIB TCP STATE iphlpapi pinvoke
// NuPkg4Src-Id: PInvoke.dbghelp.MIB_TCP_STATE
// NuPkg4Src-ContentPath: PInvoke.iphlpapi
// NuPkg4Src-Hash: SHA512Managed:7B624549AE6FCF4A6312A6BFBBA2F6B239589C6091AF2F0AD32E473D7A4A88DFB84B37B0F59B5047E4C558B8E6162954BF9D5A4D54A6EB990899F9DC09F6F487
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
        // https://msdn.microsoft.com/en-us/library/aa366896.aspx
        public enum MIB_TCP_STATE
        {
            MIB_TCP_STATE_CLOSED,
            MIB_TCP_STATE_LISTEN,
            MIB_TCP_STATE_SYN_SENT,
            MIB_TCP_STATE_SYN_RCVD,
            MIB_TCP_STATE_ESTAB,
            MIB_TCP_STATE_FIN_WAIT1,
            MIB_TCP_STATE_FIN_WAIT2,
            MIB_TCP_STATE_CLOSE_WAIT,
            MIB_TCP_STATE_CLOSING,
            MIB_TCP_STATE_LAST_ACK,
            MIB_TCP_STATE_TIME_WAIT,
            MIB_TCP_STATE_DELETE_TCB
        }
    }
}
