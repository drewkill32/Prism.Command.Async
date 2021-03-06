﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.Tasks.Annotations;

namespace Prism.Commands.Async
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
    public class DelegateCommandAsync : DelegateCommandAsyncBase
    {
        #region Private Fields


        private readonly Func<CancellationToken, Task> executeMethod;
        private Func<bool> canExecuteMethod;


        #endregion Private Fields

        
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommandAsync{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><see cref="CanExecute"/> will always return true.</remarks>
        public DelegateCommandAsync(Func<CancellationToken, Task> executeMethod)
            : this(executeMethod, () => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommandAsync{T}"/>.
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command. This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
        /// <exception cref="ArgumentNullException">When both <paramref name="executeMethod"/> and <paramref name="canExecuteMethod"/> ar <see langword="null" />.</exception>
        public DelegateCommandAsync(Func<CancellationToken, Task> executeMethod, Func<bool> canExecuteMethod)
            : base()
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), "DelegateCommand Delegates Cannot Be Null");

            cancelCommand = new CancelTaskCommand();
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        #endregion Public Constructors


        #region Public Methods



        public bool CanExecute()
        {
            return CanExecute(null) && !IsExecuting;
        }

        public async void Execute()
        {
            await ExecuteAsync();
        }

        /// <summary>
        /// Executes the command asynchronously and involves the
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public async Task ExecuteAsync()
        {
            IsExecuting = true;
            ObservableTask = new ObservableTask(executeMethod(CancelCommand.Token)) { ThrowException = this.ThrowException };
            await ObservableTask.TaskCompletion;
            IsExecuting = false;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommandAsync ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            Expression<Func<bool>> expression = Expression.Lambda<Func<bool>>(canExecuteExpression.Body);
            canExecuteMethod = expression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="TType">The type of the return value of the method that this delegate encapulates</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommandAsync ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }



        #endregion Public Methods

        #region Protected Methods

        protected override bool CanExecute(object parameter)
        {
            return canExecuteMethod() && !IsExecuting;
        }

        protected override async void Execute(object parameter)
        {
            await ExecuteAsync();
        }


        #endregion Protected Methods
    }
}