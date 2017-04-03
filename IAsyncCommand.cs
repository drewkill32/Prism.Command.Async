using System.Threading.Tasks;
using System.Windows.Input;

namespace Common.Tasks
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
    }
}
