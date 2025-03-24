using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AppStart : MonoBehaviour {

    void Awake() {
        //PlayerManager.Instance.UpdateUserData();

    }

    //TODO: AutoLogin 기능 추가

    private void Start() {
        Debug.Log("[AppStart] 앱 시작");

        PlayerPrefs.Save();

        // UI 로딩 화면 표시
        UI_Manager.Instance.Show(UI_Manager.PanelType.Loading);
        // 자동 로그인 처리
        Debug.Log("[AppStart] 자동 로그인 여부 확인 중...");
        StartCoroutine(SignInHandler.Instance.AttemptAutoSignIn((success, message) => {
            if (success) {
                Debug.Log("[AppStart] 자동 로그인 성공, 메인 화면으로 이동");
                Debug.Log(message);
                UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
            }
            else {
                Debug.Log("[AppStart] 자동 로그인 실패, 로그인 화면으로 이동");
                Debug.Log(message);
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
            }
        }));

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 자동 로그인 여부를 비동기 방식으로 확인
    /// </summary>
    /// 
    private void OnPanelRegistered(UI_Manager.PanelType panel) {
        if (panel == UI_Manager.PanelType.Login) {
            ShowLoginPanel();
            UI_Manager.Instance.OnPanelRegistered -= OnPanelRegistered; // 이벤트 해제
        }
    }

    private void ShowLoginPanel() {
        Debug.Log("[AppStart] 로그인 패널 표시 시도");
        UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
        gameObject.SetActive(true);
    }

    // public void Onclick_AppStart()
    // {

    //     UI_Manager.Instance.Show(UI_Manager.PanelType.Login);

    //     gameObject.SetActive(false);
    // }

}
