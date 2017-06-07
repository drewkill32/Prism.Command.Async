using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Commands.Async.Tests
{

    public class UnitTest1
    {

        public UnitTest1()
        {

        }
        [Fact]
        public async void CreateObservableTask()
        {

            var t = new ObservableTask<string>(Task.Run(()=>"something"));
            await t.Task;

            Assert.NotNull(t);
            Assert.NotNull(t.Task);
            Assert.NotNull(t.Task);

        }


        [Fact]
        public async void CheckResult()
        {

            var t = new ObservableTask<string>(Task.Run(() => "something"));
            Assert.Equal("something", await t.Task);

        }

        [Fact]
        public async void CheckCancel()
        {

            var t = new ObservableTask(token => Task.Delay(100000, token));
            //await Task.Delay(50);
            Assert.PropertyChanged(t,nameof(t.IsCanceled),() => t.CancelCommand.Cancel());



        }
        [Fact]
        public async void CheckStatusPropertyChanged()
        {

            var t = new ObservableTask(token => Task.Delay(50, token));
            await Assert.PropertyChangedAsync(t, nameof(t.Status), () => t.TaskCompletion);

        }

        [Fact]
        public async void CheckIsCompletedPropertyChanged()
        {

            var t = new ObservableTask(token => Task.Delay(50, token));
            await Assert.PropertyChangedAsync(t, nameof(t.IsCompleted), () => t.TaskCompletion);

        }

        [Fact]
        public async void CheckIsNotCompletedPropertyChanged()
        {

            var t = new ObservableTask(token => Task.Delay(50, token));
            await Assert.PropertyChangedAsync(t, nameof(t.IsNotCompleted), () => t.TaskCompletion);

        }

        [Fact]
        public async void CheckIsSuccessfullyCompletedPropertyChanged()
        {

            var t = new ObservableTask(token => Task.Delay(50, token));
            await Assert.PropertyChangedAsync(t, nameof(t.IsSuccessfullyCompleted), () => t.TaskCompletion);

        }
    }
}
