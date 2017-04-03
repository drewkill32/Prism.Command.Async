//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq.Expressions;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Common.Tasks.Annotations;
//using Prism.Commands;

//namespace Common.Tasks
//{
//    /// <summary>
//    /// An <see cref="Execute"/> whose delegates do not take any parameters for <see cref="CanExecute"/> and <see cref="DelegateCommandBase"/>.
//    /// </summary>
//    /// <see cref="DelegateCommand{T}"/>
//    /// <see cref="ICommand"/>
//    public class DelegateCommandAsync : DelegateCommandBase, INotifyPropertyChanged
//    {
//        #region Fields

//        private readonly Func<CancellationToken, Task> executeMethod;
//        private Func<bool> canExecuteMethod;
//        private ObservableTask execution;
//        private bool isRunning;

//        #endregion Fields

//        #region Properties

//        public bool IsRunning
//        {
//            get { return isRunning; }
//            private set
//            {
//                Set(ref isRunning, value);
//                if (Equals(isRunning, value)) return;
//                RaiseCanExecuteChanged();

//            }
//        }



//        public ObservableTask Execution
//        {
//            get { return execution; }
//            set { Set(ref execution, value); }
//        }

//        #endregion Properties



//        /// <summary>
//        /// Creates a new instance of <see cref="DelegateCommandAsync"/> with the <see cref="Action"/> to invoke on execution.
//        /// </summary>
//        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
//        public DelegateCommandAsync(Func<CancellationToken, Task> executeMethod)
//            : this(executeMethod, () => true)
//        {

//        }

//        /// <summary>
//        /// Creates a new instance of <see cref="DelegateCommandAsync"/> with the <see cref="Action"/> to invoke on execution
//        /// and a <see langword="Func" /> to query for determining if the command can execute.
//        /// </summary>
//        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
//        /// <param name="canExecuteMethod">The <see cref="Func{TResult}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
//        public DelegateCommandAsync(Func<CancellationToken, Task> executeMethod, Func<bool> canExecuteMethod)
//            : base()
//        {
//            if (executeMethod == null || canExecuteMethod == null)
//                throw new ArgumentNullException(nameof(executeMethod), "DelegateCommand delegates cannot be null");

//            this.executeMethod = executeMethod;
//            cancelCommand = new CancelAsyncCommand(RaiseCanExecuteChanged);
//            OnPropertyChanged(nameof(CancelCommand));
//            this.canExecuteMethod = canExecuteMethod;
//        }

//        ///<summary>
//        /// Executes the command.
//        ///</summary>
//        public async void Execute()
//        {
//            await ExecuteAsync();
//        }

//        /// <summary>
//        /// Executes the command async. returns an awaitable Task
//        /// </summary>
//        /// <returns></returns>
//        public async Task ExecuteAsync()
//        {
//            IsRunning = true;
//            Execution = new ObservableTask(executeMethod(cancelCommand.Token));
//            if (Execution.TaskCompletion != null)
//                await Execution.TaskCompletion;
//            IsRunning = false;
//        }

//        /// <summary>
//        /// Determines if the command can be executed.
//        /// </summary>
//        /// <returns>Returns <see langword="true"/> if the command can execute,otherwise returns <see langword="false"/>.</returns>
//        public bool CanExecute()
//        {
//            return canExecuteMethod() && !IsRunning;
//        }

//        protected override void Execute(object parameter)
//        {
//            Execute();
//        }

//        protected override bool CanExecute(object parameter)
//        {
//            return CanExecute();
//        }

//        /// <summary>
//        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
//        /// </summary>
//        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
//        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
//        /// <returns>The current instance of DelegateCommand</returns>
//        public DelegateCommandAsync ObservesProperty<T>(Expression<Func<T>> propertyExpression)
//        {
//            ObservesPropertyInternal(propertyExpression);
//            return this;
//        }

//        /// <summary>
//        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
//        /// </summary>
//        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
//        /// <returns>The current instance of DelegateCommand</returns>
//        public DelegateCommandAsync ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
//        {
//            canExecuteMethod = canExecuteExpression.Compile();
//            ObservesPropertyInternal(canExecuteExpression);
//            return this;
//        }




//        #region INotifyPropertyChanged

//        public event PropertyChangedEventHandler PropertyChanged;

//        [NotifyPropertyChangedInvocator]
//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

//        }

//        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
//        {
//            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
//            field = value;
//            OnPropertyChanged(propertyName);
//            return true;
//        }

//        #endregion INotifyPropertyChanged
//    }
//}