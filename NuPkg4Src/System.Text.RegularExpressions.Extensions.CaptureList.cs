// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: Simplifies extracting a collection of Captures from a Match
// NuPkg4Src-Tags: CSharp Source RegularExpressions Extension
// NuPkg4Src-Id: ExampleLibrary.System.Text.RegularExpressions.Extension.CaptureList
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-MakePublic: Extensions
// NuPkg4Src-Hash: SHA512Managed:374F21417309AAF90E400210CBDD4412A437FCB04D4127D9C1C75EE92468AA0AA4FB85FFB7EE35E2BFF630CC5921A8030F2B4BE957C41D3920EB7EAE6C516766
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace System.Text.RegularExpressions
{
    using System.Linq;

    internal static partial class Extensions
    {
        public static System.Collections.Generic.IEnumerable<Capture> CaptureList(this Match match, string groupName)
        {
            var captures = match.Groups[groupName].Captures;

            return Enumerable.Range(0, captures.Count)
                .Select(i => captures[i])
                .OrderBy(x => x.Index);
        }
    }
}
