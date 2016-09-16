// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: provides a collection of disposables that are disposed without having to write the usual boilerplate code
// NuPkg4Src-Tags: CSharp Source Auto Disposable
// NuPkg4Src-Id: ExampleLibrary.AutoDisposable
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-MakePublic: AutoDisposable
// NuPkg4Src-Hash: SHA512Managed:D3FE7A1C43DC040014CC6C116CE672E3B5DC99994A816BE50F4D5CBEAF37AC418488DAF29ADD746BA22113250090A0BDD20B44D19A0A5F7B8D5F5D4E196474C2
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace ExampleLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// provides a collection of disposables that are disposed without
    /// having to write the usual boilerplate code.
    /// </summary>
    internal class AutoDisposable : IDisposable
    {
        /// <summary>
        /// The disposables
        /// </summary>
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        /// <summary>
        /// Finalizes an instance of the <see cref="AutoDisposable"/> class.
        /// </summary>
        ~AutoDisposable()
        {
            Console.WriteLine("not disposed: {0}", this.GetType().FullName);
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Add the supplied disposable to the automatically disposed collection.
        /// </summary>
        /// <typeparam name="T">The type of the specific disposable</typeparam>
        /// <param name="disposable">The disposable.</param>
        /// <returns>The original disposable</returns>
        public T AddDisposable<T>(T disposable) where T : IDisposable
        {
            this.disposables.Add(disposable);
            return disposable;
        }

        /// <summary>
        /// Removes the supplied disposable from the automatically disposed collection.
        /// </summary>
        /// <typeparam name="T">The type of the specific disposable</typeparam>
        /// <param name="disposable">The disposable.</param>
        /// <returns>The original disposable</returns>
        public T RemoveDisposable<T>(T disposable) where T : IDisposable
        {
            var index = this.disposables.FindIndex(x => object.ReferenceEquals(x, disposable));
            if (index >= 0)
            {
                this.disposables.RemoveAt(index);
            }

            return disposable;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var disposable in this.disposables.Reverse<IDisposable>())
                {
                    disposable.Dispose();
                }

                this.disposables.Clear();
            }
        }
    }
}
