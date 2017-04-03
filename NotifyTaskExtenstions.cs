using System.Threading.Tasks;

namespace Common.Tasks
{
    public static class NotifyTaskExtenstions
    {

        public static ObservableTask<T> ToObservableTask<T>(this Task<T> task)
        {
            return new ObservableTask<T>(task);
        }

        public static ObservableTask ToObservableTask(this Task task)
        {
            return new ObservableTask(task);
        }

    }
}
