using System.Collections.Generic;
using UnityEngine.Events;


namespace WB
{
    public interface IPopopUI
    {
        public void Show(string msg, string okText = null, string cancelText = null, UnityAction okAction = null, UnityAction cancelAction = null);
        public void Hide();
        // public void Refresh();
    }

    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)System.Activator.CreateInstance(typeof(T), true);
                }
                return instance;
            }
        }

        protected Singleton() { }
    }

}
