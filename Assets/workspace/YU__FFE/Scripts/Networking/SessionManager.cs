using Commons;
using Commons.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace workspace.YU__FFE.Scripts.Networking {
    public class SessionManager : Singleton<SessionManager> {

        private string _accessToken;
        private string _refreshToken;

        protected override void Awake() {
            _refreshToken = GetRefreshToken();
        }

        public void UpdateTokens(string refreshToken, string accessToken) {
            UpdateRefreshToken(refreshToken);
            UpdateAccessToken(accessToken);
        }

        public void UpdateAccessToken(string token) {
            _accessToken = token;
        }

        private void UpdateRefreshToken(string token) {
            _refreshToken = token;
            PlayerPrefs.SetString("RefreshToken", token);
        }

        public string GetAccessToken() {
            return _accessToken;
        }

        public string GetRefreshToken() {
            return _refreshToken ?? PlayerPrefs.GetString("RefreshToken", string.Empty);
        }

        public void ClearTokens() {
            ClearAccessToken();
            ClearRefreshToken();
        }

        private void ClearAccessToken() {
            _accessToken = string.Empty;
        }

        private void ClearRefreshToken() {
            _refreshToken = string.Empty;
            PlayerPrefs.DeleteKey("RefreshToken");
        }

        // ======================================================
        //                     유저 토큰 검증
        // ======================================================

        // 일단 사용 X
        private IEnumerator VerifyServerSession(Action<bool> callback) {
            string url = Constants.ServerURL + "verifySession";
            WWWForm form = new WWWForm();
            form.AddField("accessToken", _accessToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<CheckResponse>(request.downloadHandler.text);
                callback(response.Success);
            }
            else {
                Debug.LogError("[SessionManager] 서버와 연결 실패.");
                callback(false);
            }
        }

        // ======================================================
        //                    유저 토큰 재발행
        // ======================================================

        public IEnumerator RefreshAccessTokenRequest(Action<bool> callback) {
            string url = $"{Constants.ServerURL}/auth/refresh";
            string refreshToken = GetRefreshToken();

            // TODO: Refresh Token 만료시 로그아웃 처리

            if (string.IsNullOrEmpty(refreshToken)) {
                callback?.Invoke(false);
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm())) {
                request.SetRequestHeader("Authorization", $"Bearer {refreshToken}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = request.downloadHandler.text;
                    var response = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                    if (response != null && !string.IsNullOrEmpty(response.AccessToken)) {
                        UpdateAccessToken(response.AccessToken);
                        callback?.Invoke(true); // 새로운 accessToken 반환
                    }
                    else {
                        callback?.Invoke(false);
                    }
                }
                else {
                    callback?.Invoke(false);
                }
            }
        }

    }
}
