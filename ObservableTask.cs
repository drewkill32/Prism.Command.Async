using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Converters;

namespace Prism.Commands
{
    public sealed class ObservableTask<TResult> : INotifyPropertyChanged
    {



        #region Properites

        public Task<TResult> Task { get; }
        public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ?
            Task.Result : default(TResult);
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
        public bool ThrowException;

        #endregion Properites

        public ObservableTask(Task<TResult> task)
        {
            Task = task;
            var _ = WatchTaskAsync(task);
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
