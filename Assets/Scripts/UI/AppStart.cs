using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AppStart : MonoBehaviour {

    void Awake() {
        //PlayerManager.Instance.UpdateUserData();
        
    }
    
    //TODO: AutoLogin 기능 추가

    private async Task Start() 
    {
        Debug.Log("[AppStart] 앱 시작");

        PlayerPrefs.Save();
        
        // UI 로딩 화면 표시
        UI_Manager.Instance.Show(UI_Manager.PanelType.Loading);

        // 데이터 로드 (자동 로그인 여부 확인)
        bool isAutoLogin = await CheckAutoLoginAsync();

        // 자동 로그인 처리
        if (isAutoLogin)
        {
            Debug.Log("[AppStart] 자동 로그인 성공, 메인 화면으로 이동");
            UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
        }
        else
        {
            Debug.Log("[AppStart] 로그인 필요, 로그인 화면으로 이동");
            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
        }
        /*// 저장된 로그인 정보 확인
        bool hasAccessToken = PlayerPrefs.HasKey("AccessToken");
        bool autoLoginEnabled = PlayerPrefs.GetInt("AutoLoginEnabledKey", 0) == 1;

        if (hasAccessToken && autoLoginEnabled)
        {
            Debug.Log("[AppStart] 자동 로그인 활성화됨, 메인 화면으로 이동");
            UIManager.Instance.Show(UIManager.PanelType.MainA);
        }
        else
        {
            Debug.Log("[AppStart] 로그인 정보 없음, 로그인 화면으로 이동");
            UIManager.Instance.Show(UIManager.PanelType.LoginA);
        }*/

        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 자동 로그인 여부를 비동기 방식으로 확인
    /// </summary>
    private async Task<bool> CheckAutoLoginAsync()
    {
        await Task.Delay(500); // 가짜 대기 시간 (실제 API 요청 시 대체 가능)

        bool hasAccessToken = PlayerPrefs.HasKey("AccessToken");
        bool autoLoginEnabled = PlayerPrefs.GetInt("AutoLoginEnabledKey", 0) == 1;

        return hasAccessToken && autoLoginEnabled;
    }
    private void OnPanelRegistered(UI_Manager.PanelType panel)
    {
        if (panel == UI_Manager.PanelType.Login)
        {
            ShowLoginPanel();
            UI_Manager.Instance.OnPanelRegistered -= OnPanelRegistered; // 이벤트 해제
        }
    }

    private void ShowLoginPanel()
    {
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
