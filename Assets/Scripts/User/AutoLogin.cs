using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLogin
{
    const string AutoLoginIDKey = "AutoLoginIdKey";
    const string AutoLoginKey = "AutoLoginKey";

    // TODO 게임 첫 시작 시 자동 로그인 시도 (초기화 후에 호출)
    public static void LastLoginUserCall()
    {
        if (PlayerPrefs.GetInt(AutoLoginKey, 1) == 1) // 기본 활성화 상태
        {
            string lastUserId = PlayerPrefs.GetString(AutoLoginIDKey, "");


            // 마지막 로그인 유저 ID 정보가 있으면
            if (!string.IsNullOrEmpty(lastUserId))
            {

                var (isSuccess, message) = SigninManager.SessionUp(lastUserId);

                if (isSuccess)
                {
                    Debug.Log($"자동 로그인 성공: {message}");
                    // TODO 로그인 성공과 동일 처리 필요
                }

                else
                {
                    Debug.Log($"자동 로그인 실패: {message}");
                    // 아무일도 없음
                }
            }
        }        
    }

    // 로그인 성공 시 유저 ID 저장
    public static void LastLoginUserSave(string userId)
    {
        PlayerPrefs.SetString(AutoLoginIDKey, userId);
        PlayerPrefs.Save(); // 변경 사항 즉시 저장
    }

    // TODO 설정쪽에서  SetAutoLogin 를 사용해서 온오프 가능

    public static void SetAutoLogin(bool isEnabled)
    {
        PlayerPrefs.SetInt(AutoLoginKey, isEnabled ? 1 : 0); // 1이면 활성화, 0이면 비활성화
        PlayerPrefs.Save();
    }
}
