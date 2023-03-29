using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnionAvatars.Utils
{
    public class TaskQueue
    {
        private SemaphoreSlim queue;
        public TaskQueue()
        {
            queue = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> task)
        {
            await queue.WaitAsync();
            try
            {
                return await task();
            }
            finally
            {
                queue.Release();
            }
        }
    }
}
