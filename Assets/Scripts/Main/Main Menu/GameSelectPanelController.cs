using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectPanelController : UI_Panel
{
    public Button btnSinglePlay;
    public Button btnMultiPlay;
    public Button btnCancel;

    void Start()
    {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.GameSelect, this);
        btnSinglePlay.onClick.AddListener(StartSinglePlay);
        btnMultiPlay.onClick.AddListener(StartMultiPlay);
        btnCancel.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    private void StartSinglePlay()
    {
        Hide();
        
        // Todo: 스타트 싱글 플레이

    }

    private void StartMultiPlay()
    {
        Hide();
        
        // Todo: 스타트 멀티 플레이

    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void OnEnable() { }
    public override void OnDisable() { }
}
