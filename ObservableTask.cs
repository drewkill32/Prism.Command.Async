using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Tasks
{

  
    public sealed class ObservableTask : ObservableTaskBase
    {
        public ObservableTask(Task task):base()
        {
            Task = task;
            if (!task.IsCompleted)
                TaskCompletion = WatchTaskAsync(task);
        }

        public ObservableTask(Func<CancellationToken, Task> command):base()
        {
            CancelCommand.NotifyCommandStarting();
            Task = command(CancelCommand.Token);
            if (!Task.IsCompleted)
                TaskCompletion = WatchTaskAsync(Task);
        }
    }
}