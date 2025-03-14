 using UnityEngine;

 public abstract class Doubleton<T> : MonoBehaviour where T : Doubleton<T> {
        private static T instanceOne;
        private static T instanceTwo;
        private static bool isShuttingDown = false;
        private static object lockObject = new object();
        private static int counter = 0;

        /// <summary>
        /// Doubleton 인스턴스에 접근 (다른 스크립트에서 사용)
        /// </summary>
        public static T Instance {
            get {
                if (isShuttingDown) {
                    return null;
                }

                lock (lockObject) {
                    if (instanceOne == null || instanceTwo == null) {
                        InitializeInstances();
                    }
                    return (counter++ % 2 == 0) ? instanceOne : instanceTwo;
                }
            }
        }

        /// <summary>
        /// 인스턴스 초기화 (씬에서 찾거나 새로 생성)
        /// </summary>
        private static void InitializeInstances() {
            T[] existingInstances = FindObjectsOfType<T>();
            if (existingInstances.Length >= 2) {
                instanceOne = existingInstances[0];
                instanceTwo = existingInstances[1];
                return;
            }

            if (existingInstances.Length == 1) {
                instanceOne = existingInstances[0];
            }

            if (instanceOne == null) {
                GameObject obj1 = new GameObject(typeof(T).Name + "_One");
                instanceOne = obj1.AddComponent<T>();
                DontDestroyOnLoad(obj1);
            }

            if (instanceTwo == null) {
                GameObject obj2 = new GameObject(typeof(T).Name + "_Two");
                instanceTwo = obj2.AddComponent<T>();
                DontDestroyOnLoad(obj2);
            }
        }

        /// <summary>
        /// 오버라이드 가능한 초기화 (자식에서 필요 시 사용)
        /// </summary>
        protected virtual void Awake() {
            if (instanceOne == null) {
                instanceOne = this as T;
                DontDestroyOnLoad(gameObject);
            } else if (instanceTwo == null) {
                instanceTwo = this as T;
                DontDestroyOnLoad(gameObject);
            } else if (instanceOne != this && instanceTwo != this) {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 앱 종료 시 해제
        /// </summary>
        protected virtual void OnApplicationQuit() {
            isShuttingDown = true;
        }

        /// <summary>
        /// 수동 해제 시
        /// </summary>
        protected void OnDestroy() {
            isShuttingDown = true;
        }
    }