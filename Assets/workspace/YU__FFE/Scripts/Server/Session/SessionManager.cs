using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.Server.Session {
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
        //                   유저 프로필 로딩 추가
        // ======================================================

        public IEnumerator GetUserProfileImage(string userId, UnityEngine.UI.Image profileImageComponent) {
            string url = $"{Constants.ServerURL}/user/profile?userId={userId}";
            string accessToken = GetAccessToken();

            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success) {
                    var response = JsonConvert.DeserializeObject<ProfileResponse>(request.downloadHandler.text);
                    if (!string.IsNullOrEmpty(response.profileUrl)) {
                        StartCoroutine(DownloadProfileImage(response.profileUrl, profileImageComponent));
                    } else {
                        Debug.LogWarning("[SessionManager] 프로필 이미지 URL 없음, 기본 이미지 적용");
                        profileImageComponent.sprite = Resources.Load<Sprite>("DefaultProfile"); // ✅ 기본 이미지 적용
                    }
                } else {
                    Debug.LogError($"[SessionManager] 프로필 이미지 불러오기 실패: {request.error}");
                    profileImageComponent.sprite = Resources.Load<Sprite>("DefaultProfile"); // ✅ 기본 이미지 적용
                }
            }
        }


        private IEnumerator DownloadProfileImage(string imageUrl, UnityEngine.UI.Image profileImageComponent) {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl)) {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success) {
                    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    profileImageComponent.sprite = sprite;
                } else {
                    Debug.LogError($"[SessionManager] 이미지 다운로드 실패: {request.error}");
                    profileImageComponent.sprite = Resources.Load<Sprite>("DefaultProfile"); // 기본 이미지 설정
                }
            }
        }


        [Serializable]
        private class ProfileResponse 
        {
            public string profileUrl;
        }
        
        // ======================================================
        //                     유저 토큰 검증
        // ======================================================

        private IEnumerator VerifyServerSession (Action<bool> callback) {
            string url = Constants.ServerURL + "verifySession";
            WWWForm form = new WWWForm();
            form.AddField("sessionToken", _accessToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<Structs.BaseResponse>(request.downloadHandler.text);
                callback(response.success);
            }
            else {
                Debug.LogError("[SessionManager] 서버와 연결 실패.");
                callback(false);
            }
        }

        // ======================================================
        //                 유저 아이디 토큰 발행
        // ======================================================

        public IEnumerator RequestNewToken(string id) {
            string url = Constants.ServerURL + "createSession";
            WWWForm form = new WWWForm();
            form.AddField("id", id);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<NetworkManager.TokenResponse>(request.downloadHandler.text);

                if (response is not null) {
                    UpdateAccessToken(response.accessToken);
                    UpdateRefreshToken(response.refreshToken); // 리프레시 토큰도 저장
                    Debug.Log("[SessionManager] 새로운 세션을 생성했습니다.");
                }
                else {
                    Debug.LogWarning($"[SessionManager] 세션 생성 실패: {response.message}");
                }
            }
            else {
                Debug.LogError("[SessionManager] 서버 연결 실패.");
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
                    var response = JsonConvert.DeserializeObject<NetworkManager.TokenResponse>(jsonResponse);
                    if (response != null && !string.IsNullOrEmpty(response.accessToken)) {
                        UpdateAccessToken(response.accessToken);
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
