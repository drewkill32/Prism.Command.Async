using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace Prism.Commands.Async
{
    public sealed class CancelTaskCommand : ICommand, INotifyPropertyChanged
    {
        #region Private Fields

        private bool commandExecuting;
        private CancellationTokenSource cts = new CancellationTokenSource();

        #endregion Private Fields

        #region Public Properties

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

        public CancellationToken Token => cts.Token;

        #endregion Public Properties

        #region Public Constructors

        public CancelTaskCommand()
        {
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler CanExecuteChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Methods

        public void Cancel()
        {
            (this as ICommand).Execute(null);
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

        public void NotifyCommandFinished()
        {
            CommandExecuting = false;

            RaiseCanExecuteChanged();
        }

        public void NotifyCommandStarting()
        {
            CommandExecuting = true;
            if (cts.IsCancellationRequested)
                cts = new CancellationTokenSource();
            RaiseCanExecuteChanged();
        }

        #endregion Public Methods

        #region Private Methods

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion Private Methods
    }
}