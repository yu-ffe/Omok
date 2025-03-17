using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.Server.Session {
    public class SessionManager : Singleton<SessionManager> {
        private const string ServerUrl = Constants.ServerURL;
        public string SessionToken { get; private set; }
        private string refreshToken; // 리프레시 토큰 저장

        // ========== 로컬 세션 저장 ========== 
        public void SetSessionToken(string token) {
            SessionToken = token;
        }

        // 리프레시 토큰을 로컬에 저장
        public void SetRefreshToken(string token) {
            refreshToken = token;
            PlayerPrefs.SetString("RefreshToken", token); // PlayerPrefs에 저장 (로컬 저장소)
        }

        // 리프레시 토큰을 로컬에서 로드
        public string GetRefreshToken() {
            return PlayerPrefs.GetString("RefreshToken", string.Empty); // 저장된 리프레시 토큰 가져오기
        }

        // ========== 서버에서 세션 검증 ========== 
        private IEnumerator VerifyServerSession(System.Action<bool> callback) {
            string url = ServerUrl + "verifySession";
            WWWForm form = new WWWForm();
            form.AddField("sessionToken", SessionToken);

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
                    SetSessionToken(response.sessionToken);
                    SetRefreshToken(response.refreshToken); // 리프레시 토큰도 저장
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
                    SetSessionToken(response.sessionToken); // 새로운 세션 토큰 설정
                    SetRefreshToken(response.refreshToken); // 새로운 리프레시 토큰 저장
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
