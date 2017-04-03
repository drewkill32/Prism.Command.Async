using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Tasks
{
    public static class AsyncCommandExt
    {
    }

    public class AsyncCommand<TResult> : AsyncCommandBase
    {
        private readonly Func<CancellationToken, Task<TResult>> command;
        private ObservableTask<TResult> execution;


        public ObservableTask<TResult> Execution
        {
            get { return execution; }
            set
            {
                if (Equals(execution, value)) return;
                execution = value;
                OnPropertyChanged();
            }
        }

        public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
        {
            this.command = command;
            cancelCommand = new CancelAsyncCommand();
        }

        public AsyncCommand(Func<CancellationToken, Task<TResult>> command, Func<bool> canExecute)
        {
            this.command = command;
            cancelCommand = new CancelAsyncCommand();
            this.canExecute = canExecute;
        }


        public override async Task ExecuteAsync()
        {
            //if (command == null) return;
            //cancelCommand.NotifyCommandStarting();
            //var t = new ObservableTask<TResult>(command(cancelCommand.Token));
            //Execution = new ObservableTask<TResult>(command(cancelCommand.Token));
            //RaiseCanExecuteChanged();
            //await Execution.TaskCompletion;
            //cancelCommand.NotifyCommandFinished();
            //RaiseCanExecuteChanged();
            Execution = null;
            cancelCommand.NotifyCommandStarting();
            IsRunning = true;
            Execution = new ObservableTask<TResult>(command(cancelCommand.Token));
            
            if (Execution.TaskCompletion != null)
                await Execution.TaskCompletion;
            cancelCommand.NotifyCommandFinished();
            OnPropertyChanged("Execution");
            IsRunning = false;
        }
    }


    public class AsyncCommand : AsyncCommandBase
    {
        private readonly Func<CancellationToken, Task> command;
        private ObservableTask execution;

        public ObservableTask Execution
        {
            get { return execution; }
            set
            {
                if (execution == value)
                {
                    return;
                }
                execution = value;
                OnPropertyChanged();
            }
        }

        public AsyncCommand(Func<CancellationToken, Task> command)
        {
            this.command = command;
            cancelCommand = new CancelAsyncCommand();
        }

        public AsyncCommand(Func<CancellationToken, Task> command, Func<bool> canExecute)
        {
            this.command = command;
            cancelCommand = new CancelAsyncCommand();
            base.canExecute = canExecute;
        }



        public override async Task ExecuteAsync()
        {
            Execution = null;
            cancelCommand.NotifyCommandStarting();
            IsRunning = true;
            Execution = new ObservableTask(command(cancelCommand.Token));
            if (Execution.TaskCompletion != null)
                await Execution.TaskCompletion;
            cancelCommand.NotifyCommandFinished();
            IsRunning = false;
        }

    }

    
}
