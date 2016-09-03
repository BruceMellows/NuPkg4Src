namespace NuPkg4Src
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal sealed class SourceFile
    {
        private readonly static HashSet<SourceConfigurationOptionType> excludedFromSourceUpdate = new HashSet<SourceConfigurationOptionType>
        {
            // SourceConfigurationOptionType.Id
        };

        private static readonly Regex OptionRegex = new Regex(
            @"^\s*// NuPkg4Src-(?<option>\w+):\s+(?<value>.*)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private SourceFile()
        {
        }

        public static SourceFile FromFullPath(string basePath, string fullPath)
        {
            basePath = Path.GetFullPath(basePath);
            fullPath = Path.GetFullPath(fullPath);

            var matchedLines = File.ReadAllLines(fullPath)
                .Select(x => Tuple.Create(x, OptionRegex.Matches(x)))
                .ToList();

            var options = matchedLines
                .TakeWhile(x => x.Item2.Count == 1)
                .Select(x => new SourceConfigurationOption(x.Item2[0].Groups["option"].Value, x.Item2[0].Groups["value"].Value))
                .Where(x => x.OptionType != SourceConfigurationOptionType.Error)
                .ToList();

            // add the Id if it does not have one
            bool addId = options.All(x => x.OptionType != SourceConfigurationOptionType.Id);
            if (addId)
            {
                options.Add(new SourceConfigurationOption(
                        SourceConfigurationOptionType.Id,
                        fullPath.Substring(0, fullPath.Length - 3).Substring(basePath.Length + 1).Replace('\\', '.')));
            }

            var lines = matchedLines.Where(x => x.Item2.Count != 1).Select(x => x.Item1).ToArray();
            var sha512 = System.Security.Cryptography.SHA512.Create();
            var hash = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes((string.Join(Environment.NewLine, lines))));
            var hashText = sha512.GetType().Name + ':' + BitConverter.ToString(hash).Replace("-", string.Empty);

            var sourceFile = options.Count > 1
                ? new SourceFile
                {
                    BasePath = basePath,
                    RelativePath = fullPath.Substring(basePath.Length + 1),
                    SourceConfigurationOptions = options,
                    Lines = lines,
                    Hash = hashText
                }
                : null;

            if (sourceFile != null)
            {
                // if it does not contain a hash or the has is wrong - update the source file
                var hashOptions = sourceFile.SourceConfigurationOptions.Where(x => x.OptionType == SourceConfigurationOptionType.Hash).ToArray();
                var noHash = hashOptions.Length == 0;
                var hashWrong = !noHash && hashOptions.Any(x => x.Value != hashText);
                if (addId || noHash || hashWrong)
                {
                    sourceFile.SourceConfigurationOptions =
                        sourceFile
                            .SourceConfigurationOptions
                            .Where(x => x.OptionType != SourceConfigurationOptionType.Hash)
                            .Concat(new[] { new SourceConfigurationOption(SourceConfigurationOptionType.Hash, sourceFile.Hash) })
                            .ToArray();

                    var version = sourceFile.SourceConfigurationOptions
                            .Where(x => x.OptionType == SourceConfigurationOptionType.Version)
                            .Select(x => Version.Parse(x.Value))
                            .SingleOrDefault();

                    if (hashWrong)
                    {
                        version = version != null
                            ? new Version(version.Major, version.Minor, version.Build + 1)
                            : new Version(1, 0, 0);
                    }
                    else if (version == null)
                    {
                        version = new Version(1, 0, 0);
                    }

                    sourceFile.SourceConfigurationOptions =
                        sourceFile.SourceConfigurationOptions
                            .Where(x => x.OptionType != SourceConfigurationOptionType.Version)
                            .Concat(new[] { new SourceConfigurationOption(SourceConfigurationOptionType.Version, version.ToString()) })
                            .ToArray();

                    File.WriteAllLines(
                        fullPath,
                        sourceFile.SourceConfigurationOptions
                            .Where(x => !excludedFromSourceUpdate.Contains(x.OptionType))
                            .Select(x => string.Format(CultureInfo.InvariantCulture, "// NuPkg4Src-{0}: {1}", x.OptionType, x.Value))
                            .Concat(sourceFile.Lines));
                }
            }

            return sourceFile;
        }

        public string BasePath { get; private set; }

        public string RelativePath { get; private set; }

        public IEnumerable<string> Lines { get; private set; }

        public string Hash { get; private set; }

        public IEnumerable<SourceConfigurationOption> SourceConfigurationOptions { get; private set; }

        public string FullPath { get { return Path.Combine(this.BasePath, this.RelativePath); } }
    }
}
