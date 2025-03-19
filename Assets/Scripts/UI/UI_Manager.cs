using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_Manager : Singleton<UI_Manager> {
    // 공통 Popup창
    public enum PanelType {
        None,
        Login,
        Main,
        Game,
        Record,
        Shop,
        Ranking,
        Option,
    }

    public UI_Popup popup;
    Dictionary<string, UnityAction> callBack;
    public Dictionary<PanelType, UI_Panel> panels;
    [SerializeField] PanelType nowShowingPanelType;

    public UI_Manager() {
        callBack = new();
        panels = new();
    }

    /// <summary> 활성화 된 패널들이 UI 매니저에 등록됨 </summary>
    public void AddPanel(PanelType key, UI_Panel panel) {
        if (key == PanelType.None)
            return;
        panels ??= new();
        if (!panels.ContainsKey(key))
            panels.Add(key, null);

        panels[key] = panel;
    }

    public void RemovePanel(PanelType key) {
        if (!panels.ContainsKey(key))
            return;

        panels.Remove(key);
    }

    /// <summary> 패널UI의 정보들을 새로고침하는 함수 등록 </summary>
    public void AddCallback(string key, UnityAction action) {
        callBack ??= new();
        if (!callBack.ContainsKey(key))
            callBack.Add(key, null);

        callBack[key] += action;
    }

    public void RequestExecute(string key) {
        if (!callBack.ContainsKey(key)) {
            Debug.Log($"[{key}]와 연결된 콜백이 없습니다.");
            return;
        }
        callBack[key].Invoke();
    }


    public void RemoveCallback(string key) {
        if (!callBack.ContainsKey(key))
            return;

        callBack[key] = null;
    }



    public void Show(PanelType panelKey) {
        if (!panels.ContainsKey(panelKey)) {
            Debug.Log("잘못된 키입니다.");
            return;
        }

        if (nowShowingPanelType != PanelType.None)
            Hide(nowShowingPanelType);

        panels[panelKey].Show();

        nowShowingPanelType = panelKey;
    }

    public void Hide(PanelType panelKey) {
        if (!panels.ContainsKey(panelKey)) {
            Debug.Log("잘못된 키입니다.");
            return;
        }
        panels[panelKey].Hide();
    }

}
