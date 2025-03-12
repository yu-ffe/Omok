using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WB
{
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
        Dictionary<string, UnityAction> callBack;
        Dictionary<string, UI_Panel> panels;




        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            callBack = new();
        }

        public void AddCallback(string key, UnityAction action)
        {
            if (!callBack.ContainsKey(key))
                callBack.Add(key, null);

            callBack[key] += action;
        }

        public void RemoveCallback(string key, UnityAction action)
        {
            if (!callBack.ContainsKey(key))
                return;

            callBack[key] -= action;
        }


        public void Show(string panelKey)
        {
            if (!panels.ContainsKey(panelKey))
            {
                //새로생성?
                return;
            }
            panels[panelKey].Show();
        }

        public void Hide(string panelKey)
        {
            if (!panels.ContainsKey(panelKey))
            {
                //새로생성?
                return;
            }
            panels[panelKey].Hide();
        }

    }
}


