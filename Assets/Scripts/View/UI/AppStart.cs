using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AppStart : MonoBehaviour {

    public GameObject canvas;
    
    private void Start()
    {
        Initialize();
    }

    public void Initialize() {
        Debug.Log("[AppStart] 앱 시작");
        
        canvas.SetActive(true);
        
        PlayerPrefs.Save();
        
        // 이미 로그인되어 있다면 자동 로그인 생략하고 메인화면 표시
        if (PlayerManager.Instance != null && PlayerManager.Instance.IsLoggedIn)
        {
            UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
            return;
        }
        
        // UI 로딩 화면 표시
        //UI_Manager.Instance.Show(UI_Manager.PanelType.Loading);
        
        // 자동 로그인 처리
        Debug.Log("[AppStart] 자동 로그인 여부 확인 중...");
        
        UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
        // 제작하는동안만 주석, 나중에 주석 풀기
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
