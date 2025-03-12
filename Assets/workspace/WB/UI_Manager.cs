using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    static UI_Manager instance;
    public static UI_Manager Get
    {
        get
        {
            if (instance == null)
                instance = new GameObject("UI_Manager").AddComponent<UI_Manager>();
            return instance;
        }
    }

    // 공통 Popup창
    public UI_Popup popup;
    Dictionary<string, List<IObserverUI>> observers;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        observers = new();
    }

    public void AddObserver(string bindingKey, IObserverUI observer)
    {
        if (!observers.ContainsKey(bindingKey))
            observers[bindingKey] = new List<IObserverUI>();

        observers[bindingKey].Add(observer);
    }

    public void OnNotifyUI(string bindingKey)
    {
        if (!observers.ContainsKey(bindingKey))
        {
            return;
        }

        for (int i = 0; i < observers[bindingKey].Count; i++)
        {
            observers[bindingKey][i].OnNotify();
        }
    }

    public void OnNotifyUI(string bindingKey, string msg)
    {
        if (!observers.ContainsKey(bindingKey))
        {
            return;
        }

        for (int i = 0; i < observers[bindingKey].Count; i++)
        {
            observers[bindingKey][i].OnNotify();
        }
    }
}

