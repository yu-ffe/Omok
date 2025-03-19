using System;
using System.Collections;
using UnityEngine;
using TMPro;
using WB;
using workspace.YU__FFE.Scripts.Server.Network;
using workspace.YU__FFE.Scripts.Server.Session;

namespace workspace.YU__FFE.Scripts.User {
    public class SignInHandler : Singleton<SignInHandler> {
        private const string AutoLoginEnabledKey = "AutoLoginEnabledKey";

        public void TrySignIn(string id, string password, Action<bool, string> callback) {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password)) {
                callback(false, "아이디와 비밀번호를 모두 입력하세요.");
                return;
            }
            SignIn(id, password, callback);
        }

        private void SignIn(string id, string pwd, Action<bool, string> callback) {
            string password = EncryptPassword(pwd);
            PlayerManager.Instance.playerData.SetPrivateData(id, password);

            StartCoroutine(NetworkManager.SignInRequest(response => {
                if (response != null) {
                    Server.Session.SessionManager.Instance.UpdateTokens(response.refreshToken, response.accessToken);
                    NetworkManager.GetUserInfoRequest(data => {
                        if (data != null) {
                            UpdateUserData(data);
                        }
                    });
                }
                callback(response != null, response?.message);
            }));
            
            PlayerManager.Instance.playerData.ClearPrivateData();
        }

        private void UpdateUserData(NetworkManager.UserDataResponse dataResponse) {
            PlayerManager.Instance.playerData.nickname = dataResponse.Nickname;
            PlayerManager.Instance.playerData.profileNum = dataResponse.ProfileNum;
            PlayerManager.Instance.playerData.coins = dataResponse.Coins;
            PlayerManager.Instance.playerData.grade = dataResponse.Grade;
            PlayerManager.Instance.playerData.rankPoint = dataResponse.RankPoint;
            PlayerManager.Instance.playerData.winCount = dataResponse.WinCount;
            PlayerManager.Instance.playerData.loseCount = dataResponse.LoseCount;
        }

        private static string EncryptPassword(string plainPassword) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return Convert.ToBase64String(plainTextBytes);
        }

        public IEnumerator AttemptAutoLogin(System.Action<bool, string> callback) {
            if (!GetAutoLoginEnabled()) {
                Debug.Log("자동 로그인 비활성화됨");
                UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
                callback(false, "자동 로그인 비활성화");
                yield break;
            }
            yield return RefreshSessionForAutoLogin(callback);
        }

        private IEnumerator RefreshSessionForAutoLogin(System.Action<bool, string> callback) {
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

        public bool GetAutoLoginEnabled() {
            return PlayerPrefs.GetInt(AutoLoginEnabledKey, 1) == 1;
        }

        public void SetAutoLoginEnabled(bool isEnabled) {
            PlayerPrefs.SetInt(AutoLoginEnabledKey, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
