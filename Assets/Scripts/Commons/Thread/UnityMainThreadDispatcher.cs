using Commons.Patterns;
using System;
using System.Collections.Generic;

namespace Commons.Thread {
    /// <summary>
    /// Unity 메인 스레드에서 실행할 작업을 큐에 넣고 처리하는 싱글톤 클래스
    /// </summary>
    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher> {
        
        // 메인 스레드에서 실행할 액션을 저장하는 큐
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

        /// <summary>
        /// 메인 스레드에서 실행할 작업을 큐에 추가하는 메서드
        /// </summary>
        /// <param name="action">실행할 액션</param>
        public static void Enqueue(Action action) {
            lock (ExecutionQueue) {
                // 액션을 큐에 추가
                ExecutionQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// 매 프레임마다 큐에 저장된 작업을 처리하는 메서드
        /// </summary>
        private void Update() {
            lock (ExecutionQueue) {
                // 큐에 저장된 모든 작업을 처리
                while (ExecutionQueue.Count > 0) {
                    // 큐에서 작업을 꺼내서 실행
                    ExecutionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}
