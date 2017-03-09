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

        private static readonly Regex DependencyRegex = new Regex(@"^(?<dependency>[^[(]+)(?<versionRange>[[(][^,]+,[^])]+[\])])?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private const string NuspecNamespaceText = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd";

        private static readonly XNamespace NuspecNamespace = NuspecNamespaceText;

        public static IEnumerable<OutputItem> Create(CommandLineOptions commandLineOptions, SourceFile sourceFile)
        {
            var tempPath = Path.Combine(commandLineOptions.TempPath, Guid.NewGuid().ToString("D"));
            Directory.CreateDirectory(tempPath);

            var result = new List<OutputItem>();
            var sourceFiles = sourceFile.GetAll().ToList();

            var id = sourceFile.SourceConfigurationOptions.Single(x => x.OptionType == SourceConfigurationOptionType.Id).Value;
            var nuspecFilename = Path.Combine(tempPath, id + ".nuspec");

            var version = sourceFile.SourceConfigurationOptions.Single(x => x.OptionType == SourceConfigurationOptionType.Version).Value;
            var nupkgFilename = Path.Combine(commandLineOptions.OutputPath, id + "." + version + ".nupkg");

            if (!File.Exists(nupkgFilename) || new FileInfo(nupkgFilename).LastWriteTimeUtc <= sourceFile.LastWriteTimeUtc())
            {
                var contentPath = sourceFile.SourceConfigurationOptions
                    .SingleOrDefault(x => x.OptionType == SourceConfigurationOptionType.ContentPath);

                if (contentPath != null)
                {
                    var fileElements = new List<XElement>();
                    sourceFiles.ForEach(x =>
                    {
                        var tempFile = Path.Combine(tempPath, Path.GetFileName(x.RelativePath));

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

                    if (commandLineOptions.Verbose)
                    {
                        result.AddRange(File.ReadAllLines(nuspecFilename).Select(OutputItem.Standard));
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

                        result.Add(OutputItem.Standard(arguments));

                        var nuspecProcess = new ConsoleExecute(nugetExe, arguments);
                        if (nuspecProcess.ExitCode != 0 && !commandLineOptions.Verbose)
                        {
                            result.AddRange(File.ReadAllLines(nuspecFilename).Select(OutputItem.Standard));
                        }
                    }
                }
            }

            Directory.Delete(tempPath, true);

            return result;
        }

        private static IEnumerable<XElement> OptionsToElements(IEnumerable<SourceConfigurationOption> options)
        {
            var dependencyValues = new List<string>();
            foreach (var option in options.Where(x => !excludedFromMetadata.Contains(x.OptionType)))
            {
                var name = option.OptionType.ToString();

                if (option.OptionType == SourceConfigurationOptionType.Dependencies)
                {
                    dependencyValues.Add(option.Value);
                }
                else
                {
                    yield return new XElement(
                        NuspecNamespace + name.Substring(0, 1).ToLowerInvariant() + name.Substring(1),
                        option.Value);
                }
            }

            if (dependencyValues.Any())
            {
                var dependenciesName = SourceConfigurationOptionType.Dependencies.ToString();
                yield return new XElement(
                    NuspecNamespace + dependenciesName.Substring(0, 1).ToLowerInvariant() + dependenciesName.Substring(1),
                    dependencyValues
                        .Select(x => DependencyRegex.Matches(x)[0])
                        .Select(x => new XElement(
                            NuspecNamespace + "dependency",
                            new XAttribute("id", x.Groups["dependency"].Value),
                            new XAttribute("version", x.Groups["versionRange"].Value))));
            }
        }
    }
}
