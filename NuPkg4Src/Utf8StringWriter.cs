// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: A StringWriter that has Encoding set to Encoding.UTF8
// NuPkg4Src-Tags: CSharp Source StringWriter UTF8
// NuPkg4Src-Id: ExampleLibrary.Utf8StringWriter
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-Hash: SHA512Managed:3E1A22887DBE0E90F58D931789A7B383AA170B56DF97A32C7F0E371A10D63F10008CE88D5C9F8692FB85D8DF08A53F2489BCA74EA631506BB61DCAD03C9AF9DA
// NuPkg4Src-Version: 1.0.0
// NO LICENSE
// ==========
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace NuPkg4Src
{
    internal class Utf8StringWriter : System.IO.StringWriter
    {
        public override System.Text.Encoding Encoding { get { return System.Text.Encoding.UTF8; } }
    }
}
