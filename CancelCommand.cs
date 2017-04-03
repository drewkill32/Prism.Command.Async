using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Tasks
{
    public sealed class CancelAsyncCommand : ICommand, INotifyPropertyChanged
    {
        private readonly Action raiseCanExecuteChanged;

        private CancellationTokenSource cts = new CancellationTokenSource();
        private bool commandExecuting;


        public CancelAsyncCommand(Action raiseCanExecuteChanged)
        {
            this.raiseCanExecuteChanged = raiseCanExecuteChanged;
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
            if (!cts.IsCancellationRequested)
                return;
            cts = new CancellationTokenSource();
            raiseCanExecuteChanged();
        }

        public void NotifyCommandFinished()
        {
            CommandExecuting = false;
            raiseCanExecuteChanged();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CommandExecuting && !cts.IsCancellationRequested;
        }

        void ICommand.Execute(object parameter)
        {
            cts.Cancel();
            raiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
