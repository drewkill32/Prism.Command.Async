using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Tasks
{
    public  class ObservableTask<TResult>:INotifyPropertyChanged
    {


        public ObservableTask(Task<TResult> task)
        {
            Task = task;
            TaskCompletion = WatchTaskAsync(task);
        }


        public ObservableTask(Func<CancellationToken, Task<TResult>> command)
        {
            cancelCommand = new CancelAsyncCommand();
            cancelCommand.NotifyCommandStarting();
            Task = command(cancelCommand.Token);
            if (!Task.IsCompleted)
                TaskCompletion = WatchTaskAsync(Task);
        }

        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ?
            Task.Result : default(TResult);



        protected CancelAsyncCommand cancelCommand;


        protected async Task<TResult> WatchTaskAsync(Task<TResult> task)
        {
            try
            {
                await task;
                cancelCommand?.NotifyCommandFinished();
            }
            catch
            {
                // ignored
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return Result;
            propertyChanged(this, new PropertyChangedEventArgs("Status"));
            propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));
            propertyChanged(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                propertyChanged(this,
                    new PropertyChangedEventArgs("InnerException"));
                propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                propertyChanged(this,
                    new PropertyChangedEventArgs("IsSuccessfullyCompleted"));  
                propertyChanged(this, new PropertyChangedEventArgs("Result"));
            }
            return Result;
        }

        public Task<TResult> TaskCompletion { get; protected set; }
        public Task<TResult> Task { get; protected set; }


        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;

        public bool IsSuccessfullyCompleted => Task.Status ==
                                               TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;

        public Exception InnerException => Exception?.InnerException;

        public string ErrorMessage => InnerException?.Message;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CancelCommand => cancelCommand;

        protected sealed class OoErrorCommand : ICommand
        {
            public bool CanExecute(object parameter)
            {
                throw new NotImplementedException();
            }

            public void Execute(object parameter)
            {
                throw new NotImplementedException();
            }

            public event EventHandler CanExecuteChanged;
        }

        protected sealed class CancelAsyncCommand : ICommand, INotifyPropertyChanged
        {

            private CancellationTokenSource cts = new CancellationTokenSource();
            private bool commandExecuting;


            public bool CommandExecuting
            {
                get { return commandExecuting; }
                set
                {
                    if (commandExecuting == value)
                    {
                        return;
                    }
                    commandExecuting = value;
                    OnPropertyChanged(nameof(CommandExecuting));
                }
            }

            public event EventHandler CanExecuteChanged;

            public CancellationToken Token => cts.Token;

            public void NotifyCommandStarting()
            {
                CommandExecuting = true;
                if (!cts.IsCancellationRequested)
                    return;
                cts = new CancellationTokenSource();
                CommandManager.InvalidateRequerySuggested();
            }

            public void NotifyCommandFinished()
            {
                CommandExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
            bool ICommand.CanExecute(object parameter)
            {
                return CommandExecuting && !cts.IsCancellationRequested;
            }
            void ICommand.Execute(object parameter)
            {
                cts.Cancel();
                CommandManager.InvalidateRequerySuggested();
            }

            public event PropertyChangedEventHandler PropertyChanged;


            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
