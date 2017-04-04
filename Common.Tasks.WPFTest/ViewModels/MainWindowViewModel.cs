using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Commands.Async;
using Prism.Mvvm;

namespace Common.Tasks.WPFTest.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        public DelegateCommandAsync AsyncCommand { get; }

        private string _title = "Prism Unity Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            AsyncCommand = new DelegateCommandAsync(AsyncTest) {ThrowException = true};
            
        }

        private async Task AsyncTest(CancellationToken arg)
        {
            await TestTask(arg);
            

        }


        private async Task<string> TestTask(CancellationToken token)
        {
            int zero = 0;
            await Task.Delay(1000, token);
            int result = 10 / zero;
            await Task.Delay(500, token);
            return "Test";

        }

    }
}
