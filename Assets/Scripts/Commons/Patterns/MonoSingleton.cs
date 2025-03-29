// 1. 싱글톤 패턴: Singleton Pattern
// 2. 추상 클래스: Abstract Class
// 3. 제네릭 클래스: Generic Class

using UnityEngine;

namespace Commons.Patterns {
    /// <summary>
    /// 싱글톤 패턴을 구현한 추상 클래스
    /// </summary>
    /// <typeparam name="T">싱글톤을 상속하는 클래스 타입</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
        
        // 싱글톤 인스턴스
        private static T _instance;
        
        // 애플리케이션 종료 여부를 확인하는 변수
        private static bool _isShuttingDown = false;
        
        // 인스턴스 생성 시 동기화 처리를 위한 객체
        private static readonly object lockObject = new object();

        /// <summary>
        /// 싱글톤 인스턴스를 반환하는 프로퍼티
        /// </summary>
        /// <returns>싱글톤 인스턴스</returns>
        public static T Instance {
            get {
                if (_instance || _isShuttingDown)
                    return _instance;
                lock (lockObject) {
                    if (_instance)
                        return _instance;
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    _instance = FindObjectOfType<T>();
                    if (_instance)
                        return _instance;
                    GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
                }
                return _instance;
            }
        }

        /// <summary>
        /// 싱글톤 인스턴스를 초기화하는 메서드
        /// </summary>
        protected virtual void Awake() {
            if (!_instance) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 애플리케이션 종료 시 호출되는 메서드
        /// </summary>
        private void OnApplicationQuit() {
            _isShuttingDown = true;
        }

        /// <summary>
        /// 오브젝트가 파괴될 때 호출되는 메서드
        /// </summary>
        private void OnDestroy() {
            _isShuttingDown = true;
        }
    }
}
