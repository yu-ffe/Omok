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
                Debug.Log("듀얼 플레이 실패: 돈 부족");
                
                Hide();

                UI_Manager.Instance.popup.Show(
                    $"코인이 부족합니다. 상점으로 가시겠습니까?",
                    "상점으로 가기",
                    "취소",
                    okAction: () =>
                    {
                        if (UI_Manager.Instance.Panels.TryGetValue(UI_Manager.PanelType.Shop, out var shopPanel))
                            shopPanel.gameObject.SetActive(true);
                    },
                    cancelAction: () => UI_Manager.Instance.popup.Hide()
                );
            }
        }));
        // Todo: 스타트 싱글 플레이

    }
    private void StartDualPlay() {
        StartCoroutine(NetworkManager.Instance.GameStartRequest(callback => {
            if(callback.Success) {
                StartCoroutine(PlayerManager.Instance.UpdateUserData());
                Hide();
                GameManager.Instance.SetGameType(Constants.GameType.DualPlayer);
                GameManager.Instance.ChangeToGameScene(Constants.GameType.DualPlayer);
            }
            else 
            {
                Debug.Log("듀얼 플레이 실패: 돈 부족");
                
                Hide();

                UI_Manager.Instance.popup.Show(
                    $"코인이 부족합니다. 상점으로 가시겠습니까?",
                    "상점으로 가기",
                    "취소",
                    okAction: () =>
                    {
                        UI_Manager.Instance.Show(UI_Manager.PanelType.Shop);
                    },
                    cancelAction: () => UI_Manager.Instance.popup.Hide()
                );
            }
        }));

    }

    private void StartMultiPlay() {
        // StartCoroutine(NetworkManager.Instance.GameStartRequest(callback => {
        //     if(callback.Success) {
        //         StartCoroutine(PlayerManager.Instance.UpdateUserData());
        //         Hide();
        //         GameManager.Instance.ChangeToGameScene(Constants.GameType.MultiPlayer);
        //     }
        //     else {
        //         Debug.Log("듀얼 플레이 실패: 돈 부족");
        //         
        //         Hide();
        //
        //         UI_Manager.Instance.popup.Show(
        //             $"코인이 부족합니다. 상점으로 가시겠습니까?",
        //             "상점으로 가기",
        //             "취소",
        //             okAction: () =>
        //             {
        //                 if (UI_Manager.Instance.Panels.TryGetValue(UI_Manager.PanelType.Shop, out var shopPanel))
        //                     shopPanel.gameObject.SetActive(true);
        //             },
        //             cancelAction: () => UI_Manager.Instance.popup.Hide()
        //         );
        //     }
        // }));
        Debug.Log("멀티 플레이 준비중");
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
