using System.Collections;
using System.Collections.Generic;
using Commons;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectPanelController : UI_Panel {
    public Button btnSinglePlay;
    public Button btnDualPlay;
    public Button btnMultiPlay;
    public Button btnCancel;

    void Start() {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.GameSelect, this);
        btnSinglePlay.onClick.AddListener(StartSinglePlay);
        btnDualPlay.onClick.AddListener(StartDualPlay);
        btnMultiPlay.onClick.AddListener(StartMultiPlay);
        btnCancel.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    private void StartSinglePlay() {
        Hide();
        GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlayer);
        // Todo: 스타트 싱글 플레이

    }
    private void StartDualPlay() {
        Hide();
        GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
        

    }

    private void StartMultiPlay() {
        Hide();
        GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlayer);

        // Todo: 스타트 멀티 플레이

    }

    public override void Show() {
        gameObject.SetActive(true);
    }

    public override void Hide() {
        gameObject.SetActive(false);
    }

    public override void OnEnable() {
    }
    public override void OnDisable() {
    }
}
