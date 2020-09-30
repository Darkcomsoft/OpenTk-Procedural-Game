using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src
{
    /// <summary>
    /// Add a Action to a queue to be invoked in next frame
    /// </summary>
    public static class NextFrameQueue
    {
        private static Queue<Action> _QueueList = new Queue<Action>();

        public static void Enqueue(Action action)
        {
            _QueueList.Enqueue(action);
        }

        public static void Tick()
        {
            while (_QueueList.Count > 0)
            {
                _QueueList.Dequeue().Invoke();
            }
        }
    }
}
