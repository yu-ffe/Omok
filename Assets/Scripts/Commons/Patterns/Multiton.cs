using System.Collections.Generic;
using UnityEngine;

namespace Commons.Patterns {
    public abstract class Multiton<T> : MonoBehaviour where T : Multiton<T> {
        private static Dictionary<string, T> instances = new Dictionary<string, T>();
        private static bool isShuttingDown = false;
        private static object lockObject = new object();

        /// <summary>
        /// 키를 기준으로 Multiton 인스턴스에 접근
        /// </summary>
        public static T GetInstance(string key) {
            if (isShuttingDown) {
                return null;
            }

            lock (lockObject) {
                if (!instances.ContainsKey(key)) {
                    instances[key] = CreateInstance(key);
                }
                return instances[key];
            }
        }

        /// <summary>
        /// 인스턴스 생성 (씬에서 찾거나 새로 생성)
        /// </summary>
        private static T CreateInstance(string key) {
            T[] existingInstances = FindObjectsOfType<T>();
            foreach (T instance in existingInstances) {
                if (!instances.ContainsValue(instance)) {
                    instances[key] = instance;
                    return instance;
                }
            }

            GameObject obj = new GameObject(typeof(T).Name + "_" + key);
            T newInstance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
            return newInstance;
        }

        /// <summary>
        /// 특정 키의 인스턴스 존재 여부 확인
        /// </summary>
        public static bool HasInstance(string key) {
            return instances.ContainsKey(key);
        }

        /// <summary>
        /// 특정 키의 인스턴스 제거
        /// </summary>
        public static void DestroyInstance(string key) {
            if (instances.ContainsKey(key)) {
                Destroy(instances[key].gameObject);
                instances.Remove(key);
            }
        }

        /// <summary>
        /// 오버라이드 가능한 초기화 (자식에서 필요 시 사용)
        /// </summary>
        protected virtual void Awake() {
            string key = gameObject.name;
            if (!instances.ContainsKey(key)) {
                instances[key] = this as T;
                DontDestroyOnLoad(gameObject);
            } else if (instances[key] != this) {
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
}