namespace workspace.YU__FFE.Scripts.Common {
    public abstract class Singleton<T> where T : Singleton<T> {
        private static T instance;
        public static T Instance {
            get {
                if (instance == null) {
                    instance = (T)System.Activator.CreateInstance(typeof(T), true);
                }
                return instance;
            }
        }

        protected Singleton() { }
    }
}
