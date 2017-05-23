using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Prism.Commands.Async
{
    public sealed class ObservableTask: INotifyPropertyChanged
    {
        private Func<CancelTaskCommand, Task> getUsers;



        #region Properites

        public Task Task { get; }
        public CancelTaskCommand CancelCommand { get; }
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
        public bool ThrowException { get; set; }
        public Task TaskCompletion { get; }

        #endregion Properites

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


        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (ThrowException)
                    throw;
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


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
