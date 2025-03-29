using Commons.Patterns;
using System;
using System.Collections.Generic;

namespace Commons.Utils {
    public class UnityMainThreadDispatcher : Singleton<UnityMainThreadDispatcher>
    {
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        public void Enqueue(Action action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}
