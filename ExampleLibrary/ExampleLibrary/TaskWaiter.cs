// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: Provides a disposable wrapper around a task that waits in the dispose
// NuPkg4Src-Tags: CSharp Source Task Wait
// NuPkg4Src-Id: ExampleLibrary.TaskWaiter
// NuPkg4Src-ExternalSourceDependencies: ExampleLibrary.AutoDisposable
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-MakePublic: TaskWaiter
// NuPkg4Src-Hash: SHA512Managed:5B214DBFE7E0DEAABFF856C748735D6D7F06F20DED6517FB9F63FF2FCEC2C4772AA89159AC5D95EF5EC90708BAFA41C3B7E85A13C7C68F044E1A878BA65656A1
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace ExampleLibrary
{
    using System;
    using System.Threading.Tasks;

    internal class TaskWaiter : AutoDisposable
    {
        private readonly Task task;

        public TaskWaiter(Action action, TaskCreationOptions creationOptions)
            : this(new Task(action, creationOptions))
        {
            this.task.Start();
        }

        public TaskWaiter(Task task)
        {
            this.AddDisposable(this.task = task);
        }

        protected override void Dispose(bool disposing)
        {
            this.task.Wait();

            base.Dispose(disposing);
        }
    }
}
