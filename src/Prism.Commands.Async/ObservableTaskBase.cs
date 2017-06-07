using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Tasks.Annotations;

namespace Prism.Commands.Async
{
    public abstract class ObservableTaskBase:INotifyPropertyChanged
    {

        public CancelTaskCommand CancelCommand { get; protected set; }
        public string ErrorMessage => InnerException?.Message;
        public AggregateException Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public bool IsCanceled => Task.IsCanceled;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsFaulted => Task.IsFaulted;
        public bool IsNotCompleted => !Task.IsCompleted;
        public bool ThrowException { get; set; }
        public bool IsSuccessfullyCompleted => Task.Status ==
                                               TaskStatus.RanToCompletion;


        public TaskStatus Status => Task.Status;
        public virtual Task Task { get; protected set; }
        public Task TaskCompletion { get; protected set; }

        

        protected virtual async Task WatchTaskAsync(Task task)
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
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(IsCompleted));
            OnPropertyChanged(nameof(IsNotCompleted));
            if (task.IsCanceled)
            {
                OnPropertyChanged(nameof(IsCanceled));
            }
            else if (task.IsFaulted)
            {
                OnPropertyChanged(nameof(IsFaulted));
                OnPropertyChanged(nameof(Exception));
                OnPropertyChanged(nameof(InnerException));
                OnPropertyChanged(nameof(ErrorMessage));
            }
            else
            {
                OnPropertyChanged(nameof(IsSuccessfullyCompleted));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
