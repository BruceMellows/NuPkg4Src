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

            if (commandLineOptions.Verbose)
            {
                Console.WriteLine("{0} {1}", System.Reflection.Assembly.GetEntryAssembly().Location, string.Join(" ", args.Select(x => '"' + x + '"')));
            }

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
                var results = sourceFiles.AsParallel().Select(x => NuPkg.Create(commandLineOptions, x)).ToList();

                results.ForEach(result => result.ToList().ForEach(Output));
            }
        }

        private static void Output(OutputItem item)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = item.Category == OutputCategory.Standard ? Console.ForegroundColor : ConsoleColor.Red;
            Console.WriteLine(item.Line);
            Console.ForegroundColor = temp;
        }

        private static int ReportException(NuPkg.DependencyParseException ex)
        {
            Output(OutputItem.Error("Dependency parse error in the following items: " + ex.Message));

            return 1;
        }

        private static int ReportException(AggregateException ex)
        {
            return ex.InnerExceptions.Sum(x => ReportGeneralException(x));
        }

        private static int ReportGeneralException(Exception ex)
        {
            var aggregateException = ex as AggregateException;
            if (aggregateException != null)
            {
                return ReportException(aggregateException);
            }

            var dependencyParseException = ex as NuPkg.DependencyParseException;
            if (dependencyParseException != null)
            {
                return ReportException(dependencyParseException);
            }

            return 0;
        }

        private static void Main(string[] args)
        {
            try
            {
                NoCatchMain(args);
            }
            catch (Exception ex)
            {
                if (0 == ReportGeneralException(ex))
                {
                    Output(OutputItem.Error(ex.ToString()));
                }
            }
        }
    }
}