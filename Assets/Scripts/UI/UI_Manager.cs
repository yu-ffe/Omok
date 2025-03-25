using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_Manager : Singleton<UI_Manager>
{
    [Serializable]
    public struct PanelData
    {
        public UI_Manager.PanelType panelType;
        public UI_Panel panel;
    }

    // 공통 Popup창
    public enum PanelType
    {
        None,
        Login,
        Main,
        Game,
        Record,
        Shop,
        Ranking,
        Option,
        Loading,
        GameSelect,
        GameEnd
    }

    public List<PanelData> panelList = new List<PanelData>();
    public Dictionary<PanelType, UI_Panel> Panels { get; private set; } = new();

    public UI_Popup popup;
    private Dictionary<string, UnityAction> _callBack = new();
    [SerializeField] PanelType nowShowingPanelType;

    public event Action<PanelType> OnPanelRegistered; // 패널이 등록될 때 발생하는 이벤트
    private bool _initialized = false;


    protected override void Awake()
    { 
        Debug.Log("[UI_Manager] Awake 시작");

       // Instance = null;
       base.Awake();
       InitializePanels(); // 패널을 한 번에 등록
        
    }

    public void InitializePanels()
    {
        Debug.Log("[UI_Manager] InitializePanels 호출");
        if (_initialized) return;
        _initialized = true;
        
        Panels.Clear(); // 혹시 남아있는 데이터 제거
        foreach (var panelData in panelList)
        {
            if (panelData.panel != null)
            {
                Panels[panelData.panelType] = panelData.panel;
                Debug.Log($"패널 등록: {panelData.panelType}");
            }
        }
    }
    /*public UI_Manager() {
        _callBack = new();
        Panels = new();
    }*/

    /// <summary> 활성화 된 패널들이 UI 매니저에 등록됨 </summary>
    public void AddPanel(PanelType key, UI_Panel panel)
    {
        if (key == PanelType.None)
            return;
        Panels ??= new();
        if (!Panels.ContainsKey(key))
            Panels.Add(key, null);

        Panels[key] = panel;
        OnPanelRegistered?.Invoke(key); // 패널이 등록되었음을 알림
    }

    public void RemovePanel(PanelType key)
    {
        if (!Panels.ContainsKey(key))
            return;

        Panels.Remove(key);
    }

    /// <summary> 패널UI의 정보들을 새로고침하는 함수 등록 </summary>
    public void AddCallback(string key, UnityAction action)
    {
        if (!_callBack.ContainsKey(key))
            _callBack[key] = action;
        else
            _callBack[key] += action;
    }

    public void RequestExecute(string key)
    {
        if (_callBack.ContainsKey(key) && _callBack[key] != null)
            _callBack[key].Invoke();
        else
            Debug.LogWarning($"[{key}]와 연결된 콜백이 없습니다.");
    }


    public void RemoveCallback(string key)
    {
        if (_callBack.ContainsKey(key))
            _callBack[key] = null;
    }


    public void Show(PanelType panelKey, bool forceRefresh = false)
    {
        if (!Panels.ContainsKey(panelKey) || !Panels[panelKey])
        {
            Debug.LogError($"Show() 실패: {panelKey} 패널이 등록되지 않음");
            return;
        }

        if (nowShowingPanelType == panelKey && Panels[panelKey].gameObject.activeSelf && !forceRefresh)
        {
            Debug.Log($"[UI_Manager] Show(): {panelKey} 이미 활성화 상태 → 중복 Show 방지");
            return;
        }

        if (nowShowingPanelType != PanelType.None)
        {
            Hide(nowShowingPanelType);
        }

        Panels[panelKey].Show();
        nowShowingPanelType = panelKey;
        Debug.Log($"[UI_Manager] Show(): {panelKey} 패널 활성화 완료");
    }




    public void Hide(PanelType panelKey)
    {
        if (Panels.ContainsKey(panelKey))
        {
            var panel = Panels[panelKey];

            // 이미 파괴된 경우 제거
            if (panel == null)
            {
                Debug.LogWarning($"패널 {panelKey}는 이미 Destroy됨. 패널 리스트에서 제거함.");
                Panels.Remove(panelKey);
                return;
            }

            panel.Hide();
        }
    }

    public bool HasPanel(PanelType panelKey)
    {
        return Panels != null && Panels.ContainsKey(panelKey);
    }



}

