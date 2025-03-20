using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectPanelController : UI_Panel {
    public Button btnSinglePlay;
    public Button btnMultiPlay;
    public Button btnCancel;

    void Start() {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.GameSelect, this);
        btnSinglePlay.onClick.AddListener(StartSinglePlay);
        btnMultiPlay.onClick.AddListener(StartMultiPlay);
        btnCancel.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    private void StartSinglePlay() {
        Hide();
        SceneManager.Instance.LoadScene("Game");
        // 게임 매니저 관련 싱글, 멀티 플레이 설정은 여기서 하면 됩니다.
        // GameManager.Instance.GameLogicInstance.SetState(new AIState());
        // Todo: 스타트 싱글 플레이

    }

    private void StartMultiPlay() {
        Hide();
        // 게임 매니저 관련 싱글, 멀티 플레이 설정은 여기서 하면 됩니다.
        SceneManager.Instance.LoadScene("Game");

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
