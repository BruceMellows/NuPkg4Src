// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: struct MINIDUMP_EXCEPTION_INFORMATION
// NuPkg4Src-Tags: CSharp Source MINIDUMP EXCEPTION INFORMATION dbghelp pinvoke
// NuPkg4Src-Id: PInvoke.dbghelp.MINIDUMP_EXCEPTION_INFORMATION
// NuPkg4Src-ContentPath: PInvoke.dbghelp
// NuPkg4Src-Hash: SHA512Managed:44A4CD45DEEBD2C7CD67E14CE7E7D49A423C4C490F3DD8EB9051BB3889004607E11173CC863C4A276CBB6FE8F99DAD1BCD9A133CFCABC997F846D7CB4107166E
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
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct MINIDUMP_EXCEPTION_INFORMATION
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }
    }
}
