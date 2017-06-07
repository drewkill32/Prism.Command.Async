using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Prism.Commands.Async
{
    public sealed class ObservableTask : ObservableTaskBase
    {



        #region Public Constructors

        public ObservableTask(Task task)
        {
            Task = task;
            TaskCompletion = WatchTaskAsync(task);
        }

        public ObservableTask(Func<CancellationToken, Task> executeMethod)
        {
            CancelCommand = new CancelTaskCommand();
            Task = executeMethod(CancelCommand.Token);
            TaskCompletion = WatchTaskAsync(Task);
        }

        #endregion Public Constructors



       
    }
}