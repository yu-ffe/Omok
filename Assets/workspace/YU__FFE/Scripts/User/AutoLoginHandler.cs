using System.Collections;
using UnityEngine;
using WB;
using workspace.YU__FFE.Scripts.Server.Session;

namespace workspace.YU__FFE.Scripts.User {
    public static class AutoLoginHandler {
        private const string AutoLoginEnabledKey = "AutoLoginEnabledKey";

        // TODO 게임 첫 시작 시 자동 로그인 시도 (초기화 후에 호출)
        public static IEnumerator AttemptAutoLogin(System.Action<bool, string> callback) {
            if (!GetAutoLoginEnabled()) {
                Debug.Log("자동 로그인 비활성화됨");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                callback(false, "자동 로그인 비활성화");
                yield break;
            }
            
            yield return RefreshSessionForAutoLogin(callback);
        }


        private static IEnumerator RefreshSessionForAutoLogin(System.Action<bool, string> callback) {
            string storedRefreshToken = Server.Session.SessionManager.Instance.GetRefreshToken();
            
            if (string.IsNullOrEmpty(storedRefreshToken)) {
                Debug.Log("리프레시 토큰이 없습니다.");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                callback(false, "리프레시 토큰이 없습니다.");
                yield break;
            }
            
            yield return Server.Session.SessionManager.Instance.RefreshAccessTokenRequest(success => {
                if (success) {
                    Debug.Log("자동 로그인 성공");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
                    callback(true, "자동 로그인 성공");
                } else {
                    Debug.Log("자동 로그인 실패");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                    callback(false, "자동 로그인 실패");
                }
            });
        }
        
        public static bool GetAutoLoginEnabled() {
            return PlayerPrefs.GetInt(AutoLoginEnabledKey, 1) == 1;
        }


        // 자동 로그인 활성화/비활성화 설정
        public static void SetAutoLoginEnabled(bool isEnabled) {
            PlayerPrefs.SetInt(AutoLoginEnabledKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
