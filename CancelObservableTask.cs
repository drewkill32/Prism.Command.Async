using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Tasks
{
    public class CancelObservableTask:ICommand
    {
        private readonly Action raiseCanExecuteChanged;
        private bool commandExecuting;
        public CancelObservableTask(Action raiseCanExecuteChanged )
        {
            this.raiseCanExecuteChanged = raiseCanExecuteChanged;
        }

        private CancellationTokenSource _cts = new CancellationTokenSource();
        

        public CancellationToken Token { get { return _cts.Token; } }
        public void NotifyCommandStarting()
        {
            commandExecuting = true;
            if (!_cts.IsCancellationRequested)
                return;
            _cts = new CancellationTokenSource();
            raiseCanExecuteChanged();
        }
        public void NotifyCommandFinished()
        {
            commandExecuting = false;
            raiseCanExecuteChanged();
        }
        public bool CanExecute(object parameter)
        {
            return commandExecuting && !_cts.IsCancellationRequested;
        }
        public void Execute(object parameter)
        {
            _cts.Cancel();
            raiseCanExecuteChanged();
        }

        public event EventHandler CanExecuteChanged;

    }
}
