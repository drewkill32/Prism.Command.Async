using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Tasks
{
    public abstract class ObservableTaskBase:INotifyPropertyChanged
    {
        protected ObservableTaskBase()
        {
            CancelCommand = new CancelAsyncCommand(RaiseCanExecuteChanged);
        }

        protected async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
                CancelCommand?.NotifyCommandFinished();
            }
            catch
            {
                // ignored
            }
            var propertyChanged = PropertyChanged;
            if (propertyChanged == null)
                return;
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
         }

        public Task TaskCompletion { get; protected set; }
        public Task Task { get; protected set; }


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


        protected CancelAsyncCommand CancelCommand { get; set; }

        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }


    }
}
