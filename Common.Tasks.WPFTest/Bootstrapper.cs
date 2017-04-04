using Microsoft.Practices.Unity;
using Prism.Unity;
using Common.Tasks.WPFTest.Views;
using System.Windows;

namespace Common.Tasks.WPFTest
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
