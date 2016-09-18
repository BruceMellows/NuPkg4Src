// NuPkg4Src-Authors: www.pinvoke.net
// NuPkg4Src-Description: UnDecorateSymbolName from dbghelp.dll
// NuPkg4Src-Tags: CSharp Source UnDecorateSymbolName dbghelp pinvoke
// NuPkg4Src-Id: PInvoke.dbghelp.UnDecorateSymbolName
// NuPkg4Src-ExternalSourceDependencies: PInvoke.dbghelp.UnDecorateFlags
// NuPkg4Src-ContentPath: PInvoke.dbghelp
// NuPkg4Src-Hash: SHA512Managed:6A2ECED3545DB8DDA06ABAB4E8D54AEDD079257C202AE9D2D50D1FAC002A82FEAE29D231ABFF13A8B84954F04DB12B6C664A3677B3EE28BEA05F196A61A039E0
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace PInvoke.dbghelp
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// <see cref="http://www.pinvoke.net/default.aspx/dbghlp32.UnDecorateSymbolName"/>
    /// </summary>
    internal static partial class NativeMethods
    {
        [DllImport("dbghelp.dll", SetLastError = true, PreserveSig = true)]
        public static extern int UnDecorateSymbolName(
            [In] [MarshalAs(UnmanagedType.LPStr)] string DecoratedName,
            [Out] StringBuilder UnDecoratedName,
            [In] [MarshalAs(UnmanagedType.U4)] int UndecoratedLength,
            [In] [MarshalAs(UnmanagedType.U4)] UnDecorateFlags Flags);
    }
}
