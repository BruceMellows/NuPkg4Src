// NO LICENSE
// ==========
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace NuPkg4Src
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal sealed class CommandLineOptions
    {
        private const string NugetOption = "-nugetpath=";
        private const string TempOption = "-tempdirectory=";
        private const string OutputPathOption = "-outputdirectory=";
        // FIXME - add temp path option - default to user/temp folder

        private readonly List<string> nonOptionArgs = new List<string>();

        public CommandLineOptions(IEnumerable<string> args)
        {
            this.NugetPath = FindExePath("nuget.exe");
            this.OutputPath = this.TempPath = Environment.CurrentDirectory;
            args.ToList().ForEach(this.Add);
        }

        private void Add(string arg)
        {
            if (!ExtractOption(arg, NugetOption, x => this.NugetPath = x)
                && !ExtractOption(arg, TempOption, x => this.TempPath = x)
                && !ExtractOption(arg, OutputPathOption, x => this.OutputPath = x))
            {
                this.nonOptionArgs.Add(arg);
            }
        }

        public IEnumerable<string> NonOptionArgs { get { return this.nonOptionArgs; } }

        public string NugetPath { get; private set; }

        public string TempPath { get; private set; }

        public string OutputPath { get; private set; }

        private static bool ExtractOption(string arg, string longOption, Action<string> assignValue)
        {
            if (!arg.ToLowerInvariant().StartsWith(longOption) || arg.Length <= longOption.Length)
                return false;

            assignValue(arg.Substring(longOption.Length));
            return true;
        }

        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="System.IO.FileNotFoundException">Raised when the exe was not found</exception>
        private static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);

            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }

                foreach (string test in Assembly.GetExecutingAssembly().GetModules().Select(x => new FileInfo(x.FullyQualifiedName).DirectoryName))
                {
                    string path = test.Trim();
                    if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                        return Path.GetFullPath(path);
                }

                return null;
                ////throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }

            return Path.GetFullPath(exe);
        }
    }
}