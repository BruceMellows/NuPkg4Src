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

    internal class Program
    {
        private static void NoCatchMain(string[] args)
        {
            CommandLineOptions commandLineOptions = new CommandLineOptions(args);

            foreach (var basePath in commandLineOptions.NonOptionArgs.Select(Path.GetFullPath).Where(Directory.Exists))
            {
                var sourceFiles = Directory.GetFiles(basePath, "*.cs", SearchOption.AllDirectories)
                    .SelectMany(x => SourceFile.FromFullPath(basePath, x))
                    .Where(x => x != null)
                    .ToList();

                sourceFiles.ForEach(x => x.UpdateDependencies(
                        sourceFiles.ToDictionary(
                            y => y.SourceConfigurationOptions.Single(z => z.OptionType == SourceConfigurationOptionType.Id).Value,
                            y => y)));

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