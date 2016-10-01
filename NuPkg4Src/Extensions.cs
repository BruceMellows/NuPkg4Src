namespace NuPkg4Src
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class Extensions
    {
        public static IEnumerable<SourceFile> GetAll(this SourceFile sourceFile)
        {
            return new[] { new[] { sourceFile } }
                .Concat(sourceFile.AssociatedSources.Select(GetAll))
                .SelectMany(x => x);
        }

        public static DateTime LastWriteTimeUtc(this SourceFile sourceFile)
        {
            return new[] { File.GetLastWriteTimeUtc(sourceFile.FullPath) }
                .Concat(sourceFile.AssociatedSources.Select(LastWriteTimeUtc))
                .Max(x => x);
        }
    }
}
