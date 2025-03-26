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
        StartCoroutine(NetworkManager.Instance.GameStartRequest(callback => {
            if(callback.Success) {
                StartCoroutine(PlayerManager.Instance.UpdateUserData());
                Hide();
                GameManager.Instance.ChangeToGameScene(Constants.GameType.SinglePlayer);
            }
            else {
                Debug.Log("싱글 플레이 실패: 돈 부족");
            }
        }));
        // Todo: 스타트 싱글 플레이

    }
    private void StartDualPlay() {
        StartCoroutine(NetworkManager.Instance.GameStartRequest(callback => {
            if(callback.Success) {
                StartCoroutine(PlayerManager.Instance.UpdateUserData());
                Hide();
                GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
            }
            else {
                Debug.Log("듀얼 플레이 실패: 돈 부족");
            }
        }));

    }

    private void StartMultiPlay() {
        StartCoroutine(NetworkManager.Instance.GameStartRequest(callback => {
            if(callback.Success) {
                StartCoroutine(PlayerManager.Instance.UpdateUserData());
                Hide();
                GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlayer);
            }
            else {
                Debug.Log("멀티 플레이 실패: 돈 부족");
            }
        }));
        // Todo: 스타트 멀티 플레이

    }

    public override void Show() {
        gameObject.SetActive(true);
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Main].gameObject.SetActive(false);
    }

    public override void Hide() {
        gameObject.SetActive(false);
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);

    }

    public override void OnEnable() {
    }
    public override void OnDisable() {
    }
}
