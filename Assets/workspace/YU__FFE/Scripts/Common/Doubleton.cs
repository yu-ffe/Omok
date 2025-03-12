using UnityEngine;

namespace workspace.YU__FFE.Scripts.Common {
    /// <summary>
    /// Generic Doubleton class 
    /// </summary>
    public abstract class Doubleton<T> : MonoBehaviour where T : Doubleton<T> {
        private static T _instanceOne;
        private static T _instanceTwo;
        private static int counter = 0;

        public static T Instance {
            get {
                if (_instanceOne == null || _instanceTwo == null) {
                    CreateInstances();
                }
                return (counter++ % 2 == 0) ? _instanceOne : _instanceTwo;
            }
        }

        private static void CreateInstances() {
            GameObject obj1 = new GameObject(typeof(T).Name + "_One");
            GameObject obj2 = new GameObject(typeof(T).Name + "_Two");

            _instanceOne = obj1.AddComponent<T>();
            _instanceTwo = obj2.AddComponent<T>();

            DontDestroyOnLoad(obj1);
            DontDestroyOnLoad(obj2);
        }

        protected Doubleton() { }
    }
}
