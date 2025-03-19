using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStart : MonoBehaviour {

    void Awake() {
        
        PlayerManager.Instance.UpdateUserData();
        // TODO: NetworkManager로 동작 변경
        // SessionManager.LoadAllSessions();
    }
    void Start() {
        //앱 초기 세팅
        Debug.Log("App Start");

        PlayerPrefs.Save();

        //TODO: AutoLogin 기능 추가

        if (true) {
            // if (AutoLogin.GetAutoLogin()) {
            // AutoLogin.LastLoginUserCall();
        }
        else {
            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
        }
        gameObject.SetActive(false);
    }

    // public void Onclick_AppStart()
    // {

    //     UI_Manager.Instance.Show(UI_Manager.PanelType.Login);

    //     gameObject.SetActive(false);
    // }

}
