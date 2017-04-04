using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace Prism.Commands.Async
{
    public sealed class CancelTaskCommand : ICommand, INotifyPropertyChanged
    {


        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool commandExecuting;


        public CancelTaskCommand()
        {
        }



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
            if (cts.IsCancellationRequested)
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

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this,EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
