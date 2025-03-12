using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WB
{
    public class UI_Manager : Singleton<UI_Manager>
    {
        // 공통 Popup창
        public UI_Popup popup;
        Dictionary<string, UnityAction> callBack;
        Dictionary<string, UI_Panel> panels;

        public UI_Manager()
        {
            callBack = new();
            callBack = new();
        }

        public void AddPanel(string key, UI_Panel panel)
        {
            if (!panels.ContainsKey(key))
                panels.Add(key, null);

            panels[key] = panel;
        }

        public void RemovePanel(string key)
        {
            if (!panels.ContainsKey(key))
                return;

            panels.Remove(key);
        }


        public void AddCallback(string key, UnityAction action)
        {
            if (!callBack.ContainsKey(key))
                callBack.Add(key, null);

            callBack[key] += action;
        }

        public void RemoveCallback(string key)
        {
            if (!callBack.ContainsKey(key))
                return;

            callBack[key] = null;
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


