// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: MiniDumpWriteDump from dbghelp.dll
// NuPkg4Src-Tags: CSharp Source MiniDumpWriteDump dbghelp pinvoke
// NuPkg4Src-Id: PInvoke.dbghelp.MiniDumpWriteDump
// NuPkg4Src-ExternalSourceDependencies: PInvoke.dbghelp.MINIDUMP_EXCEPTION_INFORMATION
// NuPkg4Src-ContentPath: PInvoke.dbghelp
// NuPkg4Src-Hash: SHA512Managed:50E8680F58AF5D731B46015877410C7D4CFF1E295462B7A4326C97051A47B30C09E4ECBD6BA5F94D8AE9B083E8A387B292824035083115D5F8A6251A3695C762
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace PInvoke.dbghelp
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// <see cref="http://www.pinvoke.net/default.aspx/dbghelp.MiniDumpWriteDump"/>
    /// </summary>
    internal static partial class NativeMethods
    {
        [DllImport("dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint ProcessId,
            IntPtr hFile,
            int DumpType,
            ref MINIDUMP_EXCEPTION_INFORMATION ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);
    }
}
