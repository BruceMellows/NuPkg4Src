namespace NuPkg4Src
{
    internal class Utf8StringWriter : System.IO.StringWriter
    {
        public override System.Text.Encoding Encoding { get { return System.Text.Encoding.UTF8; } }
    }
}
