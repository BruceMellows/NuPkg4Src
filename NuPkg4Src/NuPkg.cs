// NO LICENSE
// ==========
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace NuPkg4Src
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    internal class NuPkg
    {
        private readonly static HashSet<SourceConfigurationOptionType> excludedFromMetadata = new HashSet<SourceConfigurationOptionType>
        {
            SourceConfigurationOptionType.Hash,
            SourceConfigurationOptionType.ContentPath,
            SourceConfigurationOptionType.ExternalSourceDependencies,
            SourceConfigurationOptionType.InternalSourceDependencies,
            SourceConfigurationOptionType.MakePublic,
            SourceConfigurationOptionType.Variant,
        };

        private static readonly Regex DependencyRegex = new Regex(@"^(?<versionRange>[^,]+,[^,]+),(?<dependency>.*)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private const string NuspecNamespaceText = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

        private static readonly XNamespace NuspecNamespace = NuspecNamespaceText;

        public static void Create(CommandLineOptions commandLineOptions, SourceFile sourceFile)
        {
            var tempFiles = new List<string>();
            var sourceFiles = EnumerateSourceFiles(sourceFile).ToList();

            var id = sourceFile.SourceConfigurationOptions.Single(x => x.OptionType == SourceConfigurationOptionType.Id).Value;
            var nuspecFilename = Path.Combine(commandLineOptions.TempPath, id + ".nuspec");

            var version = sourceFile.SourceConfigurationOptions.Single(x => x.OptionType == SourceConfigurationOptionType.Version).Value;
            var nupkgFilename = Path.Combine(commandLineOptions.OutputPath, id + "." + version + ".nupkg");

            if (!File.Exists(nupkgFilename) || new FileInfo(nupkgFilename).LastWriteTimeUtc <= LastWriteTimeUtc(sourceFile))
            {
                var contentPath = sourceFile.SourceConfigurationOptions
                    .SingleOrDefault(x => x.OptionType == SourceConfigurationOptionType.ContentPath);

                if (contentPath != null)
                {
                    var fileElements = new List<XElement>();
                    sourceFiles.ForEach(x =>
                    {
                        var tempFile = Path.Combine(commandLineOptions.TempPath, Path.GetFileName(x.RelativePath));
                        tempFiles.Add(tempFile);

                        File.WriteAllLines(tempFile, x.Lines);
                        fileElements.Add(
                            new XElement(
                                NuspecNamespace + "file",
                                new XAttribute("src", tempFile),
                                new XAttribute(
                                    "target",
                                    Path.Combine(
                                    new[]
                                    {
                                        "content",
                                        contentPath.Value,
                                    }.Where(o => o != null).ToArray()))));
                    });

                    var optionElementArray = OptionsToElements(sourceFile.SourceConfigurationOptions).ToArray();
                    var document = new XDocument(
                        new XDeclaration("1.0", "UTF-8", null),
                        new XElement(NuspecNamespace + "package",
                            new XElement(NuspecNamespace + "metadata", optionElementArray),
                            new XElement(NuspecNamespace + "files",
                                fileElements.ToArray())));

                    using (var stream = new FileStream(nuspecFilename, FileMode.Create))
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        document.Save(writer);
                    }

                    // launch nuspec creator
                    var nugetExe = commandLineOptions.NugetPath;
                    if (!string.IsNullOrEmpty(nugetExe))
                    {
                        string arguments = string.Format(
                            CultureInfo.InvariantCulture,
                            "pack \"{0}\" -OutputDirectory \"{1}\" -Verbosity detailed -BasePath \"{2}\"",
                            nuspecFilename,
                            commandLineOptions.OutputPath,
                            sourceFile.BasePath);
                        Console.WriteLine(arguments);
                        var nuspecProcess = new Process
                        {
                            StartInfo =
                            {
                                FileName = nugetExe,
                                Arguments = arguments,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                            }
                        };

                        var onOutput = new DataReceivedEventHandler(new Action<object, DataReceivedEventArgs>((sender, args) => { ConsoleWriteLine(args.Data, Console.ForegroundColor); }));
                        var onError = new DataReceivedEventHandler(new Action<object, DataReceivedEventArgs>((sender, args) => { ConsoleWriteLine(args.Data, ConsoleColor.Red); }));

                        nuspecProcess.OutputDataReceived += onOutput;
                        nuspecProcess.ErrorDataReceived += onError;
                        nuspecProcess.Start();
                        nuspecProcess.BeginErrorReadLine();
                        nuspecProcess.BeginOutputReadLine();
                        nuspecProcess.WaitForExit();
                        nuspecProcess.OutputDataReceived -= onOutput;
                        nuspecProcess.ErrorDataReceived -= onError;
                    }

                    File.Delete(nuspecFilename);
                }

                tempFiles.ForEach(File.Delete);
            }
        }

        private static IEnumerable<SourceFile> EnumerateSourceFiles(SourceFile sourceFile)
        {
            return new[] { new[] { sourceFile } }
                .Concat(sourceFile.AssociatedSources.Select(x => EnumerateSourceFiles(x)))
                .SelectMany(x => x);
        }

        private static DateTime LastWriteTimeUtc(SourceFile sourceFile)
        {
            return new[] { File.GetLastWriteTimeUtc(sourceFile.FullPath) }
                .Concat(sourceFile.AssociatedSources.Select(x => LastWriteTimeUtc(x)))
                .Max(x => x);
        }

        private static void ConsoleWriteLine(string text, ConsoleColor consoleColor)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(text);
            Console.ForegroundColor = temp;
        }

        private static IEnumerable<XElement> OptionsToElements(IEnumerable<SourceConfigurationOption> options)
        {
            foreach (var option in options.Where(x => !excludedFromMetadata.Contains(x.OptionType)))
            {
                var name = option.OptionType.ToString();

                if (option.OptionType == SourceConfigurationOptionType.Dependencies)
                {
                    yield return new XElement(
                        NuspecNamespace + name.Substring(0, 1).ToLowerInvariant() + name.Substring(1),
                        option.Value
                            .Split(' ')
                            .Select(x => DependencyRegex.Matches(option.Value)[0])
                            .Select(x => new XElement(
                                NuspecNamespace + "dependency",
                                new XAttribute("id", x.Groups["dependency"].Value),
                                new XAttribute("version", x.Groups["versionRange"].Value))));
                }
                else
                {
                    yield return new XElement(
                        NuspecNamespace + name.Substring(0, 1).ToLowerInvariant() + name.Substring(1),
                        option.Value);
                }
            }
        }
    }
}
