namespace NuPkg4Src
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    public enum OutputCategory
    {
        Standard,
        Error,
    }

    public class OutputItem
    {
        private string toString;

        public OutputItem(string line, OutputCategory category)
        {
            this.Line = line;
            this.Category = category;
        }

        public string Line { get; private set; }

        public OutputCategory Category { get; private set; }

        public static OutputItem Standard(string line)
        {
            return new OutputItem(line, OutputCategory.Standard);
        }

        public static OutputItem Error(string line)
        {
            return new OutputItem(line, OutputCategory.Error);
        }

        public override string ToString()
        {
            return this.toString = (this.toString ?? (this.Category == OutputCategory.Standard ? "STD: " : "ERR: " + this.Line));
        }
    }

    public class ConsoleExecute
    {
        public ConsoleExecute(string commandPath, string arguments)
        {
            var resultLock = new object();
            var result = new List<OutputItem>();
            var process = new Process
            {
                StartInfo =
                {
                    FileName = commandPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            var onOutput = new DataReceivedEventHandler(new Action<object, DataReceivedEventArgs>((sender, args) =>
            {
                if (args != null && args.Data != null)
                {
                    lock (resultLock)
                    {
                        result.Add(OutputItem.Standard(args.Data));
                    }
                }
            }));
            var onError = new DataReceivedEventHandler(new Action<object, DataReceivedEventArgs>((sender, args) =>
            {
                if (args != null && args.Data != null)
                {
                    lock (resultLock)
                    {
                        result.Add(new OutputItem(args.Data, OutputCategory.Error));
                    }
                }
            }));

            process.OutputDataReceived += onOutput;
            process.ErrorDataReceived += onError;

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();

            process.OutputDataReceived -= onOutput;
            process.ErrorDataReceived -= onError;

            this.OutputItems = result;
            this.ExitCode = process.ExitCode;
        }

        public IEnumerable<OutputItem> OutputItems { get; private set; }

        public int ExitCode { get; private set; }
    }
}
