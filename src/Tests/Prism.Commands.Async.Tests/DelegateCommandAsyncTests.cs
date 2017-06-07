using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Commands.Async.Tests
{
    public class DelegateCommandAsyncTests
    {
        private readonly Func<CancellationToken, Task> executeMethod;
        private Func<bool> canExecuteMethod;

        public DelegateCommandAsyncTests()
        {
            executeMethod= token => Task.Delay(2000,token);
            canExecuteMethod = () => true;
        }

        [Fact]
        public void CreateNewDelegateCommandNotNull()
        {
            var cmd = new DelegateCommandAsync(executeMethod,canExecuteMethod);
            Assert.NotNull(cmd);
        }


        [Fact]
        public void ObservableTaskShouldBeNull()
        {
            var cmd = new DelegateCommandAsync(executeMethod, canExecuteMethod);
            Assert.Null(cmd.ObservableTask);
            
        }

        [Fact]
        public void CancelCommandShouldNotBeNull()
        {
            var cmd = new DelegateCommandAsync(executeMethod, canExecuteMethod);
            Assert.NotNull(cmd.CancelCommand);

        }

        [Fact]
        public void IsExecutingShouldBeFalse()
        {
            var cmd = new DelegateCommandAsync(executeMethod, canExecuteMethod);
            Assert.False(cmd.IsExecuting);

        }

        [Fact]
        public async void PropertyChangedIsExecuting()
        {
            var cmd = new DelegateCommandAsync(executeMethod, canExecuteMethod);
            await Assert.PropertyChangedAsync(cmd, nameof(cmd.IsExecuting), () => cmd.ExecuteAsync());
            Assert.False(cmd.IsExecuting);

        }

        [Fact]
        public async void IsExecutingTrueAndFalse()
        {
            var cmd = new DelegateCommandAsync(executeMethod, canExecuteMethod);
            Assert.False(cmd.IsExecuting);
            var t= cmd.ExecuteAsync();
            Assert.True(cmd.IsExecuting);
            await t;
            Assert.False(cmd.IsExecuting);

        }


        [Fact]
        public async void ShouldCancel()
        {
            var cmd = new DelegateCommandAsync(executeMethod, canExecuteMethod);
            cmd.ExecuteAsync();
            cmd.Cancel();
            Assert.True(cmd.ObservableTask.IsCanceled);



        }
    }
}
