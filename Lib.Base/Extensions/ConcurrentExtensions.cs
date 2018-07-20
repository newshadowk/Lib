using System.Collections.Concurrent;

namespace Lib.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConcurrentExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="concurrentQueue"></param>
        public static void Clear<T>(this ConcurrentQueue<T> concurrentQueue)
        {
            while (!concurrentQueue.IsEmpty)
            {  
                T remove;
                concurrentQueue.TryDequeue(out remove);
            }
        }

        /// <summary>
        /// Clear all items.
        /// </summary>
        public static void Clear<T>(this ConcurrentBag<T> concurrentBag)
        {
            while (!concurrentBag.IsEmpty)
            {
                T someItem;
                concurrentBag.TryTake(out someItem);
            }
        }
    }
}
