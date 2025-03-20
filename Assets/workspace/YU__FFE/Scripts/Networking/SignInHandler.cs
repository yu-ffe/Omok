using Commons.Models;
using System;
using System.Collections;
using UnityEngine;
using workspace.YU__FFE.Scripts.Commons;

namespace workspace.YU__FFE.Scripts.Networking {
    
    public class SignInHandler : Singleton<SignInHandler> {
        
        private const string AutoLoginEnabled = "AutoLoginEnabledKey";

        public void AttemptSignIn(string id, string password, Action<bool, string> callback) {

            if (!ValidationManager.Validate("email", id, callback)) return;
            if (!ValidationManager.Validate("password", password, callback)) return;

            UtilityManager.EncryptPassword(ref password);

            StartCoroutine(SignIn(id, password, callback));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator SignIn(string id, string password, Action<bool, string> callback) {

            PlayerManager.Instance.playerData.SetPrivateData(id, password);

            TokenResponse tokenResponse = null;
            yield return NetworkManager.SignInRequest(response => {
                tokenResponse = response;
                PlayerManager.Instance.playerData.ClearPrivateData();
            });
            if (tokenResponse is null) {
                callback(false, "로그인 실패");
                yield break;
            }
            SessionManager.Instance.UpdateTokens(tokenResponse.RefreshToken, tokenResponse.AccessToken);

            yield return LoadPlayerData(callback);
        }

        public IEnumerator AttemptAutoLogin(Action<bool, string> callback) {
            
            if (!GetAutoLoginEnabled()) {
                callback(false, "자동 로그인 비활성화");
                yield break;
            }
            
            TokenResponse tokenResponse = null;
            yield return NetworkManager.TryAutoLoginRequest(response => {
                tokenResponse = response;
            });
            if (tokenResponse is null) {
                callback(false, "자동 로그인 실패");
                yield break;
            }
            SessionManager.Instance.UpdateTokens(tokenResponse.RefreshToken, tokenResponse.AccessToken);
            callback(true, "자동 로그인 성공");
            
            yield return LoadPlayerData(callback);
        }
        
        private static IEnumerator LoadPlayerData(Action<bool, string> callback) {
            PlayerDataResponse playerInfoResponse = null;
            yield return NetworkManager.GetUserInfoRequest(response => { playerInfoResponse = response; });
            if (playerInfoResponse is null) {
                callback(false, "사용자 정보 요청 실패");
                yield break;
            }

            PlayerManager.Instance.SetPlayerData(playerInfoResponse);
            callback(true, "로그인 성공");
        }

        private static bool GetAutoLoginEnabled() {
            return PlayerPrefs.GetInt(AutoLoginEnabled, 1) == 1;
        }

        public void SetAutoLoginEnabled(bool isEnabled) {
            PlayerPrefs.SetInt(AutoLoginEnabled, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
