using System.Collections;
using UnityEngine;
using WB;
using workspace.YU__FFE.Scripts.Server.Session;

namespace workspace.YU__FFE.Scripts.User {
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
                    // 세션 갱신 시도
                    yield return RefreshSessionForAutoLogin(userId, callback);
                }
                else {
                    Debug.Log($"로그인 기록 없음");
                    // 로그인 기록이 없으면 Login 화면 패널로 전환
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                    callback(false, "로그인 기록 없음");
                }
            }
            else {
                // 자동 로그인 비활성화
                Debug.Log("자동 로그인 비활성화됨");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                callback(false, "자동 로그인 비활성화");
            }
        }

        private static IEnumerator RefreshSessionForAutoLogin(string userId, System.Action<bool, string> callback) {
            string storedRefreshToken = Server.Session.SessionManager.Instance.GetRefreshToken();

            // 리프레시 토큰이 없으면 자동 로그인 실패
            if (string.IsNullOrEmpty(storedRefreshToken)) {
                Debug.Log("리프레시 토큰이 없습니다.");
                callback(false, "리프레시 토큰이 없습니다.");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                yield break;
            }

            // 세션 갱신 시도
            yield return Server.Session.SessionManager.Instance.RefreshSession((success) => {
                if (success) {
                    // 세션 갱신 성공
                    Debug.Log("자동 로그인 성공");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Main);
                    callback(true, "자동 로그인 성공");
                } else {
                    // 세션 갱신 실패
                    Debug.Log("자동 로그인 실패");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                    callback(false, "자동 로그인 실패");
                }
            });
        }

        public static bool GetAutoLoginEnabled() {
            return PlayerPrefs.GetInt(AutoLoginEnabledKey, 1) == 1;
        }

        // 로그인 성공 시 유저 정보 저장
        public static void SaveUserIdForAutoLogin(string userId) {
            PlayerPrefs.SetString(AutoLoginIdKey, userId);
            PlayerPrefs.Save();
        }

        // 자동 로그인 활성화/비활성화 설정
        public static void SetAutoLoginEnabled(bool isEnabled) {
            PlayerPrefs.SetInt(AutoLoginEnabledKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
