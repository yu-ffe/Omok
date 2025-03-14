using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLogin
{
    // 게임 시작 시 자동 로그인 시도 (초기화 후에 호출)
    public void LastLoginUserCall()
    {
        string lastUserId = PlayerPrefs.GetString("AutoLoginIdKey", null);


        // 마지막 로그인 유저 ID 정보가 있으면
        if (!string.IsNullOrEmpty(lastUserId))
        {
            /*
            var (isSuccess, message) = SigninManager.TryLogin(lastUserId, LoadSavedPassword(lastUserId));

            if (isSuccess)
            {
                Debug.Log($"자동 로그인 성공: {message}");
                SceneManager.LoadScene("MainScene"); // 메인 화면 이동
            }
            else
            {
                Debug.Log($"자동 로그인 실패: {message}");
                // 로그인 UI 활성화 (필요한 경우)
            }*/
        }
    }

    // 로그인 한 유저 세이브 (playerpref)
    public void LastLoginUserSave()
    {
        PlayerPrefs.SetString("AutoLoginIdKey", "UserId");
    }



}
