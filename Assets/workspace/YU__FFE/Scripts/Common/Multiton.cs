namespace workspace.YU__FFE.Scripts.Common {
    using System.Collections.Generic;
    using UnityEngine;

    namespace workspace.YU__FFE.Scripts.Common {
        /// <summary>
        /// 2개 이상의 인스턴스를 가질 수 있는 클래스
        /// </summary>
        public abstract class Multiton<T> : MonoBehaviour where T : Multiton<T> {
            private static Dictionary<string, T> _instances = new Dictionary<string, T>();

            public static T Instance(string key) {
                if (!_instances.ContainsKey(key)) {
                    _instances[key] = CreateInstance(key);
                }
                return _instances[key];
            }

            private static T CreateInstance(string key) {
                GameObject obj = new GameObject(typeof(T).Name + "_" + key);
                T instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
                return instance;
            }

            public static bool HasInstance(string key) {
                return _instances.ContainsKey(key);
            }

            public static void DestroyInstance(string key) {
                if (_instances.ContainsKey(key)) {
                    Destroy(_instances[key].gameObject);
                    _instances.Remove(key);
                }
            }

            protected Multiton() { }
        }
    }

}
