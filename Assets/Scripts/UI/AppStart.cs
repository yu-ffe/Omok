using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppStart : MonoBehaviour {

    void Awake() {
        PlayerManager.Instance.UpdateUserData();
    }
    void Start() {
        //앱 초기 세팅
        Debug.Log("App Start");

        PlayerPrefs.Save();

        //TODO: AutoLogin 기능 추가

        SignInHandler.Instance.AttemptAutoLogin((success, message) => {
            if(success){
                UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
            }
            else{
                Debug.Log("자동 로그인 실패: " + message);
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
            }
        });
        gameObject.SetActive(false);
    }

    // public void Onclick_AppStart()
    // {

    //     UI_Manager.Instance.Show(UI_Manager.PanelType.Login);

    //     gameObject.SetActive(false);
    // }

}
