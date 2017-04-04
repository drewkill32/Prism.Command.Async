using System;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Commands;
using Prism.Commands.Async;
using Prism.Mvvm;
using Xunit;
using Assert = Xunit.Assert;

namespace Common.Tasks.Tests
{
    /// <summary>
    /// Summary description for DelegateCommandFixture
    /// </summary>
    [TestClass]
    public class DelegateCommandFixture : BindableBase
    {
        [Fact]
        [TestMethod]
        public void WhenConstructedWithGenericTypeOfObject_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommandAsync<object>(param => { });

            // verify
            Assert.NotNull(actual);
        }

        [Fact]
        [TestMethod]
        public void WhenConstructedWithGenericTypeOfNullable_InitializesValues()
        {
            // Prepare

            // Act
            var actual = new DelegateCommandAsync<int?>(param => { });

            // verify
            Assert.NotNull(actual);
        }

        [Fact]
        [TestMethod]
        public void WhenConstructedWithGenericTypeIsNonNullableValueType_Throws()
        {
            Assert.Throws<InvalidCastException>(() =>
            {
                var actual = new DelegateCommandAsync<int>(param => { });
            });
        }

        [Fact]
        [TestMethod]
        public void ExecuteCallsPassedInExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommandAsync<object>(handlers.Execute);
            object parameter = new object();

            command.Execute(parameter);

            Assert.Same(parameter, handlers.ExecuteParameter);
        }

        [Fact]
        [TestMethod]
        public void CanExecuteCallsPassedInCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommandAsync<object>(handlers.Execute, handlers.CanExecute);
            object parameter = new object();

            handlers.CanExecuteReturnValue = true;
            bool retVal = command.CanExecute(parameter);

            Assert.Same(parameter, handlers.CanExecuteParameter);
            Assert.Equal(handlers.CanExecuteReturnValue, retVal);
        }

        [Fact]
        [TestMethod]
        public void CanExecuteReturnsTrueWithouthCanExecuteDelegate()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommandAsync<object>(handlers.Execute);

            bool retVal = command.CanExecute(null);

            Assert.Equal(true, retVal);
        }

        [Fact]
        [TestMethod]
        public void RaiseCanExecuteChangedRaisesCanExecuteChanged()
        {
            var handlers = new DelegateHandlers();
            var command = new DelegateCommandAsync<object>(handlers.Execute);
            bool canExecuteChangedRaised = false;
            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            command.RaiseCanExecuteChanged();

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void CanRemoveCanExecuteChangedHandler()
        {
            var command = new DelegateCommandAsync<object>((o) => { });

            bool canExecuteChangedRaised = false;

            EventHandler handler = (s, e) => canExecuteChangedRaised = true;

            command.CanExecuteChanged += handler;
            command.CanExecuteChanged -= handler;
            command.RaiseCanExecuteChanged();

            Assert.False(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void ShouldPassParameterInstanceOnExecute()
        {
            bool executeCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommandAsync<MyClass>(delegate (MyClass parameter)
            {
                Assert.Same(testClass, parameter);
                executeCalled = true;
            });

            command.Execute(testClass);
            Assert.True(executeCalled);
        }

        [Fact]
        [TestMethod]
        public void ShouldPassParameterInstanceOnCanExecute()
        {
            bool canExecuteCalled = false;
            MyClass testClass = new MyClass();
            ICommand command = new DelegateCommandAsync<MyClass>((p) => { }, delegate (MyClass parameter)
            {
                Assert.Same(testClass, parameter);
                canExecuteCalled = true;
                return true;
            });

            command.CanExecute(testClass);
            Assert.True(canExecuteCalled);
        }

        [Fact]
        [TestMethod]
        public void ShouldThrowIfAllDelegatesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandAsync<object>(null, null);
            });
        }


        [Fact]
        [TestMethod]
        public void ShouldThrowIfExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandAsync<object>(null);
            });
        }

        [Fact]
        [TestMethod]
        public void ShouldThrowIfCanExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandAsync<object>((o) => { }, null);
            });
        }

        [Fact]
        [TestMethod]
        public void DelegateCommandShouldThrowIfAllDelegatesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(null, null);
            });
        }

        [Fact]
        [TestMethod]
        public void DelegateCommandShouldThrowIfExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(null);
            });
        }

        [Fact]
        [TestMethod]
        public void DelegateCommandGenericShouldThrowIfCanExecuteMethodDelegateNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommandAsync<object>((o) => { }, null);
            });
        }

        [Fact]
        [TestMethod]
        public void IsActivePropertyIsFalseByDeafult()
        {
            var command = new DelegateCommandAsync<object>(DoNothing);
            Assert.False(command.IsActive);
        }

        [Fact]
        [TestMethod]
        public void IsActivePropertyChangeFiresEvent()
        {
            bool fired = false;
            var command = new DelegateCommandAsync<object>(DoNothing);
            command.IsActiveChanged += delegate { fired = true; };
            command.IsActive = true;

            Assert.True(fired);
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandExecuteShouldInvokeExecuteAction()
        {
            bool executed = false;
            var command = new DelegateCommand(() => { executed = true; });
            command.Execute();

            Assert.True(executed);
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandCanExecuteShouldInvokeCanExecuteFunc()
        {
            bool invoked = false;
            var command = new DelegateCommand(() => { }, () => { invoked = true; return true; });

            bool canExecute = command.CanExecute();

            Assert.True(invoked);
            Assert.True(canExecute);
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldDefaultCanExecuteToTrue()
        {
            var command = new DelegateCommand(() => { });
            Assert.True(command.CanExecute());
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateThrowsIfDelegatesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(null, null);
            });
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandThrowsIfExecuteDelegateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(null);
            });
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandThrowsIfCanExecuteDelegateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var command = new DelegateCommand(() => { }, null);
            });
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldObserveCanExecute()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommand(() => { }).ObservesCanExecute(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldObserveCanExecuteAndObserveOtherProperties()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommand(() => { }).ObservesCanExecute(() => BoolProperty).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            canExecuteChangedRaised = false;
            Assert.False(canExecuteChangedRaised);

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldNotObserveDuplicateCanExecute()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ICommand command = new DelegateCommand(() => { }).ObservesCanExecute(() => BoolProperty).ObservesCanExecute(() => BoolProperty);
            });
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldObserveOneProperty()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldObserveMultipleProperties()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);

            canExecuteChangedRaised = false;

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandShouldNotObserveDuplicateProperties()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DelegateCommand command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => IntProperty);
            });
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand(() => { }).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            RaisePropertyChanged(null);

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void NonGenericDelegateCommandNotObservingPropertiesShouldNotRaiseOnEmptyPropertyName()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommand(() => { });

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            RaisePropertyChanged(null);

            Assert.False(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandShouldObserveCanExecute()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommandAsync<object>((o) => { }).ObservesCanExecute(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandWithNullableParameterShouldObserveCanExecute()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommandAsync<int?>((o) => { }).ObservesCanExecute(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandShouldObserveCanExecuteAndObserveOtherProperties()
        {
            bool canExecuteChangedRaised = false;

            ICommand command = new DelegateCommandAsync<object>((o) => { }).ObservesCanExecute(() => BoolProperty).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            Assert.False(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
            Assert.False(command.CanExecute(null));

            canExecuteChangedRaised = false;
            Assert.False(canExecuteChangedRaised);

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
            Assert.True(command.CanExecute(null));
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandShouldNotObserveDuplicateCanExecute()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ICommand command =
                    new DelegateCommandAsync<object>((o) => { }).ObservesCanExecute(() => BoolProperty)
                        .ObservesCanExecute(() => BoolProperty);
            });
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandShouldObserveOneProperty()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommandAsync<object>((o) => { }).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandShouldObserveMultipleProperties()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommandAsync<object>((o) => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => BoolProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            IntProperty = 10;

            Assert.True(canExecuteChangedRaised);

            canExecuteChangedRaised = false;

            BoolProperty = true;

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandShouldNotObserveDuplicateProperties()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                DelegateCommandAsync<object> commandAsync = new DelegateCommandAsync<object>((o) => { }).ObservesProperty(() => IntProperty).ObservesProperty(() => IntProperty);
            });
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandObservingPropertyShouldRaiseOnEmptyPropertyName()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommandAsync<object>((o) => { }).ObservesProperty(() => IntProperty);

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            RaisePropertyChanged(null);

            Assert.True(canExecuteChangedRaised);
        }

        [Fact]
        [TestMethod]
        public void GenericDelegateCommandNotObservingPropertiesShouldNotRaiseOnEmptyPropertyName()
        {
            bool canExecuteChangedRaised = false;

            var command = new DelegateCommandAsync<object>((o) => { });

            command.CanExecuteChanged += delegate { canExecuteChangedRaised = true; };

            RaisePropertyChanged(null);

            Assert.False(canExecuteChangedRaised);
        }



        private bool _boolProperty;
        public bool BoolProperty
        {
            get { return _boolProperty; }
            set { SetProperty(ref _boolProperty, value); }
        }

        private int _intProperty;
        public int IntProperty
        {
            get { return _intProperty; }
            set { SetProperty(ref _intProperty, value); }
        }

        class CanExecutChangeHandler
        {
            public bool CanExeucteChangedHandlerCalled;
            public void CanExecuteChangeHandler(object sender, EventArgs e)
            {
                CanExeucteChangedHandlerCalled = true;
            }
        }

        public void DoNothing(object param)
        { }


        class DelegateHandlers
        {
            public bool CanExecuteReturnValue = true;
            public object CanExecuteParameter;
            public object ExecuteParameter;

            public bool CanExecute(object parameter)
            {
                CanExecuteParameter = parameter;
                return CanExecuteReturnValue;
            }

            public void Execute(object parameter)
            {
                ExecuteParameter = parameter;
            }
        }
    }

    internal class MyClass
    {
    }
}