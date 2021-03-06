﻿// NO LICENSE
// ==========
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

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
            SourceConfigurationOptionType.Variant,
        };

        private readonly static HashSet<SourceConfigurationOptionType> excludedFromSourceRead = new HashSet<SourceConfigurationOptionType>
        {
            SourceConfigurationOptionType.Variant,
            SourceConfigurationOptionType.Error,
        };

        private static readonly Regex CSharpCommentRegex = new Regex(@"^\s*//\s*(?<commentBody>.*)$", RegexOptions.Compiled);
        private static readonly Regex XamlCommentRegex = new Regex(@"^\s*<!--\s*(?<commentBody>.*)-->\s*$", RegexOptions.Compiled);
        private static readonly Regex OptionRegex = new Regex(@"^NuPkg4Src-(?<option>\w+):\s+(?<value>.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private const string regexPartIdentifier = @"(?<identifier>[a-zA-Z_][a-zA-Z0-9_]*)";
        private const string regexPartModifiers = @"((?<modifier>new|public|protected|internal|private|abstract|sealed|static)\s+)+";
        private const string regexPartPartialOpt = @"((?<partial>partial)\s+)?";
        private const string regexPartClass = @"(?<class>class|interface)\s+";
        private const string regexPartClassName = "(?<className>" + regexPartIdentifier + @")\s*";
        private const string regexPartGenericType = @"(?<genericType>" + regexPartIdentifier + @")\s*";
        private const string regexPartGenerics = @"(?<generics><\s*" + regexPartGenericType + @"(,\s*" + regexPartGenericType + @")*>)?\s*";

        private static readonly Regex ClassRegex = new Regex(@"^\s*" + regexPartModifiers + regexPartPartialOpt + regexPartClass + regexPartClassName + regexPartGenerics + ".*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private SourceFile()
        {
        }

        public static IEnumerable<SourceFile> FromXamlSource(string basePath, string fullPath)
        {
            return FromSource(
                Path.GetFullPath(basePath),
                Path.GetFullPath(fullPath),
                false,
                XamlCommentRegex,
                () => FromCSharpSource(basePath, Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath) + ".cs"), true).ToArray(),
                x => "<!-- " + x + " -->");
        }

        public static IEnumerable<SourceFile> FromCSharpSource(string basePath, string fullPath)
        {
            return FromCSharpSource(basePath, fullPath, false);
        }

        private static IEnumerable<SourceFile> FromCSharpSource(string basePath, string fullPath, bool isAssociatedSource)
        {
            return FromSource(
                Path.GetFullPath(basePath),
                Path.GetFullPath(fullPath),
                isAssociatedSource,
                CSharpCommentRegex,
                () => Enumerable.Empty<SourceFile>(),
                x => "// " + x);
        }

        public string BasePath { get; private set; }

        public string RelativePath { get; private set; }

        public IEnumerable<string> Lines { get; private set; }

        public IEnumerable<SourceFile> AssociatedSources { get; private set; }

        public string Hash { get; private set; }

        public IEnumerable<SourceConfigurationOption> SourceConfigurationOptions { get; private set; }

        public string FullPath { get { return Path.Combine(this.BasePath, this.RelativePath); } }

        public DateTime LastWriteTimeUtc { get; private set; }

        public string GetOption(SourceConfigurationOptionType optionType)
        {
            var option = this.SourceConfigurationOptions.SingleOrDefault(x => x.OptionType == optionType);
            return option != null ? option.Value : null;
        }

        public void UpdateDependencies(Dictionary<string, SourceFile> sourceFileLookup)
        {
            var internalSourceDependencyIds = this.SourceConfigurationOptions
                .Where(x => x.OptionType == SourceConfigurationOptionType.InternalSourceDependencies)
                .Select(x => x.Value.Split(' '))
                .SelectMany(x => x)
                .ToList();

            var variant = this.GetOption(SourceConfigurationOptionType.Variant);

            var externalSourceDependencyIds = this.SourceConfigurationOptions
                .Where(x => x.OptionType == SourceConfigurationOptionType.ExternalSourceDependencies)
                .Select(x => x.Value.Split(' ').Select(y => !string.IsNullOrEmpty(variant) ? string.Join(".", y, variant) : y))
                .SelectMany(x => x)
                .ToList();

            var sourceDependencies = internalSourceDependencyIds
                .Concat(externalSourceDependencyIds)
                .Select(x =>
                {
                    var baseFile = x.Split(',').First();
                    if (x.Length != baseFile.Length)
                    {
                        // version supplied
                        return x;
                    }

                    // create version to match this files version
                    var options = sourceFileLookup[baseFile].SourceConfigurationOptions;
                    var version = Version.Parse(options.Single(y => y.OptionType == SourceConfigurationOptionType.Version).Value);

                    return string.Format(
                        "{0}[{1},{2})",
                        options.Single(y => y.OptionType == SourceConfigurationOptionType.Id).Value,
                        version,
                        new Version(version.Major, version.Minor + 1));
                })
                .ToList();

            var dependencies = this.SourceConfigurationOptions
                    .Where(x => x.OptionType == SourceConfigurationOptionType.Dependencies)
                    .Select(x => x.Value.Split(' '))
                    .SelectMany(x => x)
                    .Concat(sourceDependencies)
                    .Where(x => !string.IsNullOrEmpty(x));

            this.SourceConfigurationOptions = this.SourceConfigurationOptions
               .Where(x => x.OptionType != SourceConfigurationOptionType.Dependencies)
               .Concat(dependencies.Select(x => new SourceConfigurationOption(SourceConfigurationOptionType.Dependencies, x)))
               .ToList();
        }

        private static IEnumerable<SourceFile> FromSource(
            string basePath,
            string fullPath,
            bool isAssociatedSource,
            Regex regex,
            Func<IEnumerable<SourceFile>> getAssociatedSources,
            Func<string, string> formatComment)
        {
            var matchedLines = File.ReadAllLines(fullPath).Select(
                    line =>
                    {
                        var commentMatches = regex.Matches(line);
                        return Tuple.Create(
                            line,
                            commentMatches.Count == 1
                                ? OptionRegex.Matches(commentMatches[0].Groups["commentBody"].Value.Trim())
                                : commentMatches);
                    });

            var options = matchedLines
                .TakeWhile(x => x.Item2.Count == 1)
                .Select(x => new SourceConfigurationOption(x.Item2[0].Groups["option"].Value, x.Item2[0].Groups["value"].Value))
                .Where(x => !excludedFromSourceRead.Contains(x.OptionType))
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

            var sourceFile = isAssociatedSource || options.Count > 1
                ? new SourceFile
                {
                    BasePath = basePath,
                    RelativePath = fullPath.Substring(basePath.Length + 1),
                    SourceConfigurationOptions = options,
                    Lines = lines,
                    LastWriteTimeUtc = new FileInfo(fullPath).LastWriteTimeUtc,
                    AssociatedSources = getAssociatedSources(),
                }
                : null;

            if (sourceFile == null)
            {
                yield break;
            }

            var sourceFiles = sourceFile.GetAll().ToList();
            var sha512 = System.Security.Cryptography.SHA512.Create();
            var hash = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes((string.Join(Environment.NewLine, sourceFiles.Select(x => x.Lines).SelectMany(x => x)))));
            var hashText = sha512.GetType().Name + ':' + BitConverter.ToString(hash).Replace("-", string.Empty);
            sourceFile.Hash = hashText;

            // if the source has the markup that identifies it as a SourceFile (!isAssociatedSource)
            // AND does not contain a hash
            // AND the hash is wrong
            // THEN update the source file
            var hashOptions = sourceFile.SourceConfigurationOptions.Where(x => x.OptionType == SourceConfigurationOptionType.Hash).ToArray();
            var noHash = hashOptions.Length == 0;
            var hashWrong = !isAssociatedSource && !noHash && hashOptions.Any(x => x.Value != hashText);
            if (!isAssociatedSource && (addId || noHash || hashWrong))
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
                        .Select(x => formatComment(string.Format(CultureInfo.InvariantCulture, "NuPkg4Src-{0}: {1}", x.OptionType, x.Value)))
                        .Concat(sourceFile.Lines));

                sourceFile.LastWriteTimeUtc = new FileInfo(fullPath).LastWriteTimeUtc;
            }

            // FIXME - probably should not yield return sourceFile when the above write-back was not required
            // FIXME - how can we tell if the variants need to be yield returned or not
            yield return sourceFile;

            var makePublicClassNames = new HashSet<string>(
                sourceFile.SourceConfigurationOptions
                    .Where(x => x.OptionType == SourceConfigurationOptionType.MakePublic)
                    .SelectMany(x => x.Value.Split(' ').Where(y => !string.IsNullOrWhiteSpace(y))));

            var makePublicPatches = sourceFile.Lines
                .Select(line => Tuple.Create(line, ClassRegex.Matches(line)))
                .Where(x => x.Item2.Count != 0 && makePublicClassNames.Contains(x.Item2[0].Groups["className"].Value))
                .Select(x => Tuple.Create(x.Item1, x.Item2[0].CaptureList("modifier").SingleOrDefault(y => y.Value == "internal" || y.Value == "private")))
                .Select(x => Tuple.Create(x.Item1, x.Item2 != null ? x.Item1.Substring(0, x.Item2.Index) + "public" + x.Item1.Substring(x.Item2.Index + x.Item2.Length) : string.Empty))
                .ToDictionary(x => x.Item1, x => x.Item2);

            var patched = false;
            var patchedSourceFile = new SourceFile
            {
                BasePath = sourceFile.BasePath,
                RelativePath = sourceFile.RelativePath,
                Lines = sourceFile.Lines.Select(x =>
                {
                    var key = makePublicPatches.Keys.SingleOrDefault(y => string.Equals(x, y));
                    if (key == null)
                    {
                        return x;
                    }

                    patched = true;
                    return makePublicPatches[key];
                }).ToList(),
                Hash = sourceFile.Hash,
                SourceConfigurationOptions = sourceFile.SourceConfigurationOptions,
                LastWriteTimeUtc = sourceFile.LastWriteTimeUtc,
                AssociatedSources = sourceFile.AssociatedSources,
            };

            if (patched)
            {
                var idOption = new SourceConfigurationOption(
                            SourceConfigurationOptionType.Id,
                            patchedSourceFile.SourceConfigurationOptions
                                .Single(x => x.OptionType == SourceConfigurationOptionType.Id).Value + ".public");
                var variantOption = new SourceConfigurationOption(SourceConfigurationOptionType.Variant, "public");
                patchedSourceFile.SourceConfigurationOptions = patchedSourceFile.SourceConfigurationOptions
                    .Where(x => x.OptionType != SourceConfigurationOptionType.Id && x.OptionType != SourceConfigurationOptionType.Variant)
                    .Concat(new[] { idOption, variantOption })
                    .ToList();

                yield return patchedSourceFile;
            }

            if (sourceFile.SourceConfigurationOptions.Any(x => x.OptionType == SourceConfigurationOptionType.ExternalSourceDependencies))
            {
                var nodepSourceFile = new SourceFile
                {
                    BasePath = sourceFile.BasePath,
                    RelativePath = sourceFile.RelativePath,
                    Lines = sourceFile.Lines.ToList(),
                    Hash = sourceFile.Hash,
                    SourceConfigurationOptions = sourceFile.SourceConfigurationOptions,
                    LastWriteTimeUtc = sourceFile.LastWriteTimeUtc,
                    AssociatedSources = sourceFile.AssociatedSources,
                };

                // nodep
                var idOption = new SourceConfigurationOption(
                    SourceConfigurationOptionType.Id,
                    nodepSourceFile.SourceConfigurationOptions.Single(x => x.OptionType == SourceConfigurationOptionType.Id).Value + ".nodep");
                var variantOption = new SourceConfigurationOption(SourceConfigurationOptionType.Variant, "nodep");
                nodepSourceFile.SourceConfigurationOptions = nodepSourceFile.SourceConfigurationOptions
                    .Where(x => x.OptionType != SourceConfigurationOptionType.Id
                        && x.OptionType != SourceConfigurationOptionType.Variant
                        && x.OptionType != SourceConfigurationOptionType.ExternalSourceDependencies)
                    .Concat(new[] { idOption, variantOption })
                    .ToList();

                yield return nodepSourceFile;
            }
        }

        public override string ToString()
        {
            return this.RelativePath;
        }
    }
}
