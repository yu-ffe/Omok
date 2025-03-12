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
        [SerializeField] string nowShowingPanelKey;

        public UI_Manager()
        {
            callBack = new();
            callBack = new();
        }


        /// <summary> 활성화 된 패널들이 UI 매니저에 등록됨 </summary>
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

        /// <summary> 패널UI의 정보들을 새로고침하는 함수 등록 </summary>
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
                Debug.Log("잘못된 키입니다.");
                return;
            }
            panels[panelKey].Show();

            Hide(nowShowingPanelKey);

            nowShowingPanelKey = panelKey;
        }

        public void Hide(string panelKey)
        {
            if (!panels.ContainsKey(panelKey))
            {
                Debug.Log("잘못된 키입니다.");
                return;
            }
            panels[panelKey].Hide();
        }

    }
}


