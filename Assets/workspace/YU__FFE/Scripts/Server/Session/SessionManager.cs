using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.Server.Session {
    public class SessionManager : Singleton<SessionManager> {
        private const string ServerUrl = Constants.ServerURL;
        private string _accessToken;
        private string _refreshToken; // 리프레시 토큰 저장

        private void Awake() {
            _refreshToken = GetRefreshToken();
        }
        
        // ========== 로컬 세션 저장 ========== 
        public void UpdateSessionToken(string token) {
            _accessToken = token;
        }

        // 리프레시 토큰을 로컬에 저장
        public void UpdateRefreshToken(string token) {
            _refreshToken = token;
            PlayerPrefs.SetString("RefreshToken", token); // PlayerPrefs에 저장 (로컬 저장소)
        }

        // 리프레시 토큰을 로컬에서 로드
        public string GetRefreshToken() {
            return _refreshToken ?? PlayerPrefs.GetString("RefreshToken", string.Empty);        }
        
        public void UpdateAccessToken(string token) {
            this._accessToken = token; //
        }
        public string GetAccessToken() {
            return _accessToken; // 저장된 리프레시 토큰 가져오기
        }

        
        public void UpdateTokens(string refreshToken, string sessionToken) {
            if (!string.IsNullOrEmpty(refreshToken)) {
                Server.Session.SessionManager.Instance.UpdateRefreshToken(refreshToken);
                Debug.Log("리프레시 토큰 저장 완료");
            }

            if (!string.IsNullOrEmpty(sessionToken)) {
                Server.Session.SessionManager.Instance.UpdateSessionToken(sessionToken);
                Debug.Log("세션 토큰 저장 완료");
            }
        }
        
        // ========== 서버에서 세션 검증 ========== 
        private IEnumerator VerifyServerSession(System.Action<bool> callback) {
            string url = ServerUrl + "verifySession";
            WWWForm form = new WWWForm();
            form.AddField("sessionToken", _accessToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<BaseResponse>(request.downloadHandler.text);
                callback(response.success);
            }
            else {
                Debug.LogError("[SessionManager] 서버와 연결 실패.");
                callback(false);
            }
        }

        // ========== 서버에서 새 세션 요청 ========== 
        public IEnumerator RequestNewSession(string id) {
            string url = ServerUrl + "createSession";
            WWWForm form = new WWWForm();
            form.AddField("id", id);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<SessionResponse>(request.downloadHandler.text);

                if (response.success) {
                    UpdateSessionToken(response.sessionToken);
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

        // ========== 리프레시 토큰을 사용하여 세션 갱신 ========== 
        public IEnumerator RefreshSession(System.Action<bool> callback) {
            string storedRefreshToken = GetRefreshToken();
            if (string.IsNullOrEmpty(storedRefreshToken)) {
                Debug.LogError("[SessionManager] 리프레시 토큰이 없습니다.");
                callback(false);
                yield break;
            }

            string url = ServerUrl + "refreshSession";
            WWWForm form = new WWWForm();
            form.AddField("refreshToken", storedRefreshToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                var response = JsonUtility.FromJson<SessionResponse>(request.downloadHandler.text);

                if (response.success) {
                    UpdateSessionToken(response.sessionToken); // 새로운 세션 토큰 설정
                    UpdateRefreshToken(response.refreshToken); // 새로운 리프레시 토큰 저장
                    Debug.Log("[SessionManager] 세션 갱신 성공.");
                    callback(true);
                }
                else {
                    Debug.LogWarning("[SessionManager] 세션 갱신 실패: " + response.message);
                    callback(false);
                }
            }
            else {
                Debug.LogError("[SessionManager] 서버와 연결 실패.");
                callback(false);
            }
        }
    }

    // ========== 서버 응답 모델 ========== 
    [System.Serializable]
    public class BaseResponse {
        public bool success;
        public string message;
    }

    [System.Serializable]
    public class SessionResponse : BaseResponse {
        public string sessionToken;
        public string refreshToken; // 리프레시 토큰 추가
    }
}
