using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Prism.Commands.Async
{
    public sealed class ObservableTask<TParam> : ObservableTaskBase
    {

        #region Public Constructors

        public new Task<TParam> Task { get; }
        public TParam Result => Task.Result;


        public ObservableTask(Task<TParam> task)
        {
            Task = task;
            TaskCompletion = WatchTaskAsync(task);
        }


        public ObservableTask(Func<CancellationToken, Task<TParam>> executeMethod)
        {
            CancelCommand = new CancelTaskCommand();
            Task = executeMethod(CancelCommand.Token);
            TaskCompletion = WatchTaskAsync(Task);
        }


        protected override async Task WatchTaskAsync(Task task)
        {
            await base.WatchTaskAsync(task);
            OnPropertyChanged(nameof(Result));
        }

        #endregion Public Constructors

    }
}