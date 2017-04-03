using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;

namespace Common.Tasks
{
    public abstract class AsyncCommandBase: IAsyncCommand,INotifyPropertyChanged
    {

        public abstract Task ExecuteAsync();
        protected CancelAsyncCommand cancelCommand; 
        protected  Func<bool> canExecute;
        private bool isRunning;

        #region Properties


        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                if (isRunning == value)
                {
                    return;
                }
                isRunning = value;
                RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public ICommand CancelCommand => cancelCommand;

        #endregion Properties


        public virtual async void Execute()
        {
                 await ExecuteAsync();
        }

        public event EventHandler CanExecuteChanged;

        public static void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }



        protected sealed class CancelAsyncCommand : ICommand,INotifyPropertyChanged
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
                    OnPropertyChanged();
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
                RaiseCanExecuteChanged();
            }

            public void NotifyCommandFinished()
            {
                CommandExecuting = false;
                RaiseCanExecuteChanged();
            }

            bool ICommand.CanExecute(object parameter)
            {
                return CommandExecuting && !cts.IsCancellationRequested;
            }

            void ICommand.Execute(object parameter)
            {
                cts.Cancel();
                RaiseCanExecuteChanged();
            }

            public event PropertyChangedEventHandler PropertyChanged;


            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public virtual bool CanExecute(object parameter)
        {
            return canExecute == null ? !IsRunning : canExecute.Invoke() && !IsRunning ;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Execute(object parameter)
        {
            Execute();
        }
    }
}