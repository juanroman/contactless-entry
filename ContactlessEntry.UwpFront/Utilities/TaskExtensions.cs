using System;
using System.Threading.Tasks;

namespace ContactlessEntry.UwpFront.Utilities
{
    public static class TaskExtensions
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, Action<Exception> exceptionHandler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                exceptionHandler?.Invoke(exception);
            }
        }
    }
}
