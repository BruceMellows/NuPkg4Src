// NO LICENSE
// ==========
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace NuPkg4Src
{
    using System;
    using System.IO;
    using System.Linq;

    internal class Program
    {
        private static void NoCatchMain(string[] args)
        {
            CommandLineOptions commandLineOptions = new CommandLineOptions(args);

            foreach (var basePath in commandLineOptions.NonOptionArgs.Select(Path.GetFullPath).Where(Directory.Exists))
            {
                var csharpSourceFiles = Directory.GetFiles(basePath, "*.cs", SearchOption.AllDirectories)
                    .SelectMany(x => SourceFile.FromCSharpSource(basePath, x))
                    .ToArray();

                var xamlSourceFiles = Directory.GetFiles(basePath, "*.xaml", SearchOption.AllDirectories)
                    .SelectMany(x => SourceFile.FromXamlSource(basePath, x))
                    .ToArray();

                var sourceFiles = csharpSourceFiles.Concat(xamlSourceFiles).Where(x => x != null).ToList();

                sourceFiles.ForEach(x => x.UpdateDependencies(
                        sourceFiles.ToDictionary(
                            y => y.SourceConfigurationOptions.Single(z => z.OptionType == SourceConfigurationOptionType.Id).Value,
                            y => y)));

                // FIXME - can do this in parallel
                sourceFiles.ForEach(x => NuPkg.Create(commandLineOptions, x));
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                NoCatchMain(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}