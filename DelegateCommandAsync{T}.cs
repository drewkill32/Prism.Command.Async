using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Tasks;
using Common.Tasks.Annotations;
using Prism.Commands;

// ReSharper disable once CheckNamespace
namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    /// <typeparam name="T">Parameter type.</typeparam>
    /// <remarks>
    /// The constructor deliberately prevents the use of value types.
    /// Because ICommand takes an object, having a value type for T would cause unexpected behavior when CanExecute(null) is called during XAML initialization for command bindings.
    /// Using default(T) was considered and rejected as a solution because the implementor would not be able to distinguish between a valid and defaulted values.
    /// <para/>
    /// Instead, callers should support a value type by using a nullable value type and checking the HasValue property before using the Value property.
    /// <example>
    ///     <code>
    /// public MyClass()
    /// {
    ///     this.submitCommand = new DelegateCommand&lt;int?&gt;(this.Submit, this.CanSubmit);
    /// }
    /// 
    /// private bool CanSubmit(int? customerId)
    /// {
    ///     return (customerId.HasValue &amp;&amp; customers.Contains(customerId.Value));
    /// }
    ///     </code>
    /// </example>
    /// </remarks>
    public class DelegateCommandAsync<T> : DelegateCommandBase,INotifyPropertyChanged
    {
        private readonly Func<CancellationToken,Task<T>> executeMethod;
        private ObservableTask<T> observableTask;
        private Func<T, bool> canExecuteMethod;
        private CancelObservableTaskCommand cancelCommand;

        public ObservableTask<T> ObservableTask
        {
            get { return observableTask; }
            set
            {
                if (observableTask == value) return;
                observableTask = value;
                OnPropertyChanged();
            }
        }


        public CancelObservableTaskCommand CancelCommand => cancelCommand;


        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommandAsync{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><see cref="CanExecute"/> will always return true.</remarks>
        public DelegateCommandAsync(Func<CancellationToken,Task<T>> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommandAsync{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <exception cref="ArgumentNullException">When both <paramref name="executeMethod"/> and <paramref name="canExecuteMethod"/> ar <see langword="null" />.</exception>
        public DelegateCommandAsync(Func<CancellationToken,Task<T>> executeMethod, Func<T, bool> canExecuteMethod)
            : base()
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod),"DelegateCommand Delegates Cannot Be Null");

            TypeInfo genericTypeInfo = typeof(T).GetTypeInfo();

            // DelegateCommand allows object or Nullable<>.  
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericTypeInfo.IsValueType)
            {
                if ((!genericTypeInfo.IsGenericType) || (!typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(genericTypeInfo.GetGenericTypeDefinition().GetTypeInfo())))
                {
                    throw new InvalidCastException("DelegateCommand Invalid Generic Payload Type");
                }
            }
            cancelCommand = new CancelObservableTaskCommand(RaiseCanExecuteChanged);
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        ///<summary>
        ///Executes the command and invokes the <see cref="Action{Task{T}}"/> provided during construction.
        ///</summary>
        ///<param name="parameter">Data used by the command.</param>
        public void Execute(T parameter)
        {
            executeMethod(parameter);
        }

        /// <summary>
        /// Executes the command asynchronously and involes the 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(T parameter)
        {
            ObservableTask = new ObservableTask<T>(executeMethod(cancelCommand.Token));
            RaiseCanExecuteChanged();
            await ObservableTask.TaskCompletion;
        }

        ///<summary>
        ///Determines if the command can execute by invoked the <see cref="Func{T,Bool}"/> provided during construction.
        ///</summary>
        ///<param name="parameter">Data used by the command to determine if it can execute.</param>
        ///<returns>
        ///<see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        ///</returns>
        public bool CanExecute(T parameter)
        {
            return canExecuteMethod(parameter);
        }

        protected override void Execute(object parameter)
        {
            Execute((T)parameter);
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="TType">The type of the return value of the method that this delegate encapulates</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommandAsync<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommandAsync<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(canExecuteExpression.Body, Expression.Parameter(typeof(T), "o"));
            canExecuteMethod = expression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}