using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WB
{
    public class AppStart : MonoBehaviour
    {

        void Awake()
        {
            SessionManager.LoadAllSessions();
        }
        void Start()
        {
            //앱 초기 세팅
            Debug.Log("App Start");
            
            if (AutoLogin.GetAutoLogin())
            {
                AutoLogin.LastLoginUserCall(); // 자동 로그인 시도
            }
            else
            {
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login); // 수동 로그인
            }
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 저장된 토큰을 사용한 자동 로그인 시도
        /// </summary>
        private void TryAutoLogin()
        {
            // 저장된 토큰 가져오기
            string token = PlayerPrefs.GetString("USER_TOKEN", "");

            if (!string.IsNullOrEmpty(token))
            {

                // 실제 로그인 로직 호출 (예: AuthManager 사용)
                // AuthManager.Instance.LoginWithToken(token);
                // 토큰 존재 시 바로 메인으로 이동
                UI_Manager.Instance.Show(UI_Manager.PanelType.Main); // 실제 로그인 성공 시 호출 위치
            }
            else
            {
                // 토큰 없을 때 로그인 화면 표시
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
            }
        }
        

        // public void Onclick_AppStart()
        // {

        //     UI_Manager.Instance.Show(UI_Manager.PanelType.Login);

        //     gameObject.SetActive(false);
        // }

    }
}
