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
        // Todo: 싱글플레이게임(AI와 두기) 연결
    }

    private void StartMultiPlay()
    {
        Hide();
        // Todo: 멀티플레이게임 연결
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
    }

    public override void OnDisable()
    {
    }
}
