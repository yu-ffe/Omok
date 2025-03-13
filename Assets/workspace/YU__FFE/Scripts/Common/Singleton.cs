using UnityEngine;


namespace workspace.YU__FFE.Scripts.Common {
    /// <summary>
    /// Generic Singleton class 
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
        private static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = (T)System.Activator.CreateInstance(typeof(T), true);
                }
                return _instance;
            }
        }

        protected Singleton() { }
    }
}
