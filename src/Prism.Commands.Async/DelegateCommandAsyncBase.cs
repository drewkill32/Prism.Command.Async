using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Tasks.Annotations;

namespace Prism.Commands.Async
{
    public abstract class DelegateCommandAsyncBase: DelegateCommandBase, INotifyPropertyChanged
    {

        protected CancelTaskCommand cancelCommand;
        protected bool isExecuting;
        protected ObservableTask observableTask;
        protected bool throwException;

        #region Public Properties

        public CancelTaskCommand CancelCommand => cancelCommand;

        public bool IsExecuting
        {
            get { return isExecuting; }
            protected set
            {
                if (isExecuting == value) return;
                isExecuting = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
                if (isExecuting)
                    cancelCommand.NotifyCommandStarting();
                else
                    cancelCommand.NotifyCommandFinished();
            }
        }

        public ObservableTask ObservableTask
        {
            get { return observableTask; }
            protected set
            {
                if (observableTask == value) return;
                observableTask = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        public bool ThrowException
        {
            get { return throwException; }
            set
            {
                if (throwException == value) return;
                throwException = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties


        public void ThrowIfFaulted()
        {
            if (ObservableTask == null)
                return;
            if (ObservableTask.IsFaulted)
                throw ObservableTask.Exception;
        }

        public void Cancel()
        {
            CancelCommand?.Cancel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
