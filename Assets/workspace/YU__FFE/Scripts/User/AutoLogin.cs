using WB;

namespace workspace.YU__FFE.Scripts.User {
    using System.Collections;
    using UnityEngine;

    public static class AutoLogin {
        private const string AutoLoginIdKey = "AutoLoginIdKey";
        private const string AutoLoginEnabledKey = "AutoLoginEnabledKey";

        // TODO 게임 첫 시작 시 자동 로그인 시도 (초기화 후에 호출)
        public static IEnumerator AttemptAutoLogin(System.Action<bool, string> callback) {
            bool isAutoLoginEnabled = GetAutoLoginEnabled(); // 기본 활성화
            
            if (isAutoLoginEnabled) {
                string userId = PlayerPrefs.GetString(AutoLoginIdKey, "");

                // 마지막 로그인 유저 ID 정보가 있으면
                if (!string.IsNullOrEmpty(userId)) {
                    var (isSuccess, message) = SigninManager.SessionUp(userId);

                    if (isSuccess) {
                        Debug.Log($"자동 로그인 성공: {message}");
                        // 자동 로그인 성공 시 Main 화면 패널로 전환
                        UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
                    }
                    else {
                        Debug.Log($"자동 로그인 실패: {message}");
                        // 자동 로그인 실패 시 Login 화면 패널로 전환
                        UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                    }
                }
                else {
                    Debug.Log($"로그인 기록 없음");
                    // 로그인 기록이 없으면 Login 화면 패널로 전환
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                }
            }
            yield return null;
        }


        public static bool GetAutoLoginEnabled() {
            return PlayerPrefs.GetInt(AutoLoginEnabledKey, 1) == 1;
        }

        // LastLoginUserSave -> SaveUserIdForAutoLogin 함수 명 변경
        // 로그인 성공 시 유저 정보 저장
        public static void SaveUserIdForAutoLogin(string userId) {
            PlayerPrefs.SetString(AutoLoginIdKey, userId);
            PlayerPrefs.Save();
        }

        // SetAutoLogin -> SetAutoLoginEnabled 함수 명 변경
        // 자동 로그인 활성화/비활성화 설정
        public static void SetAutoLoginEnabled(bool isEnabled) {
            PlayerPrefs.SetInt(AutoLoginEnabledKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
