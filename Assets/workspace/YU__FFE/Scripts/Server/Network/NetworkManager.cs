using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using workspace.YU__FFE.Scripts.Common;
using workspace.YU__FFE.Scripts;
using workspace.YU__FFE.Scripts.User;

namespace workspace.YU__FFE.Scripts.Server.Network {
    public class NetworkManager : Singleton<NetworkManager> {

        private const int MaxRetryCount = 3; // 연결 실패시 최대 재시도 횟수

        // ======================================================
        //                        회원가입
        // ======================================================

        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator SignUpRequest(Action<TokenResponse> callback) {
            string url = $"{Constants.ServerURL}/auth/signup";
            PlayerData playerData = PlayerManager.Instance.playerData;

            WWWForm form = new WWWForm();
            form.AddField("id", playerData.id);
            form.AddField("password", playerData.password);
            form.AddField("nickname", playerData.nickname);
            form.AddField("profileNum", playerData.profileNum);

            using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
                yield return request.SendWebRequest();

                bool success = request.result == UnityWebRequest.Result.Success;
                string responseText = request.downloadHandler.text;

                callback?.Invoke(success ? JsonConvert.DeserializeObject<TokenResponse>(responseText) : null);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private static IEnumerator CheckDuplicateRequest(string type, string value, Action<CheckResponse> callback) {
            string url = $"{Constants.ServerURL}/auth/signup/check";
            WWWForm form = new WWWForm();
            form.AddField("type", type); // 'nickname' or 'id' as type
            form.AddField("value", value);

            using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
                yield return request.SendWebRequest();

                callback?.Invoke(JsonConvert.DeserializeObject<CheckResponse>(request.downloadHandler.text));
            }
        }

        public static IEnumerator CheckIdRequest(string id, Action<CheckResponse> callback) {
            yield return CheckDuplicateRequest("id", id, callback);
        }

        public static IEnumerator CheckNicknameRequest(string nickname, Action<CheckResponse> callback) {
            yield return CheckDuplicateRequest("nickname", nickname, callback);
        }

        // ======================================================
        //                         로그인
        // ======================================================

        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator SignInRequest(Action<TokenResponse> callback) {
            string url = $"{Constants.ServerURL}/auth/signin";
            PlayerData playerData = PlayerManager.Instance.playerData;

            WWWForm form = new WWWForm();
            form.AddField("id", playerData.id);
            form.AddField("password", playerData.password);

            using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
                yield return request.SendWebRequest();

                string responseText = request.downloadHandler.text;
                callback?.Invoke(JsonConvert.DeserializeObject<TokenResponse>(responseText));
            }
        }

        public static IEnumerator TryAutoLogin(Action<TokenResponse> callback) {
            string url = $"{Constants.ServerURL}/auth/signin/refresh";
            string refreshToken = Session.SessionManager.Instance.GetRefreshToken();
            // TODO: refresh token 만료 처리 필요할 수 있음

            using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "")) {
                request.SetRequestHeader("Authorization", "Bearer " + refreshToken);
                yield return request.SendWebRequest();

                string jsonResponse = request.downloadHandler.text;
                callback?.Invoke(JsonConvert.DeserializeObject<TokenResponse>(jsonResponse));
            }
        }

        // ======================================================
        //                        로그아웃
        // ======================================================

        public static IEnumerator LogOutRequest(Action<bool> callback) {
            string url = $"{Constants.ServerURL}/logout";
            string refreshToken = Session.SessionManager.Instance.GetRefreshToken();

            if (string.IsNullOrEmpty(refreshToken)) {
                callback?.Invoke(false);
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm())) {
                request.SetRequestHeader("Authorization", $"Bearer {refreshToken}");
                yield return request.SendWebRequest();

                callback?.Invoke(request.result == UnityWebRequest.Result.Success);
            }
        }

        // ======================================================
        //                     유저 데이터
        // ======================================================

        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator GetUserInfoRequest(Action<UserDataResponse> callback) {
            string url = $"{Constants.ServerURL}/user/info";
            string accessToken = Session.SessionManager.Instance.GetAccessToken();
            int retryCount = 0;

            while (retryCount < MaxRetryCount) {
                using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                    request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success) {
                        string jsonResponse = request.downloadHandler.text;
                        UserDataResponse userDataResponse = JsonConvert.DeserializeObject<UserDataResponse>(jsonResponse);
                        callback?.Invoke(userDataResponse);
                        yield break;
                    }

                    if (request.responseCode != 401)
                        continue;
                    yield return Session.SessionManager.Instance.RefreshAccessTokenRequest((success) => {
                        if (!success) {
                            callback?.Invoke(null);
                            // TODO: 실패 로직 처리 가능성 있음
                        }
                    });
                    retryCount++;
                }
            }
            callback?.Invoke(null);
        }
        
        public static void GetRanksRequest(Action<object> action) {
            throw new NotImplementedException();
        }

        // =========================Send=========================
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator SendGameResult(bool isWin) {
            string url = $"{Constants.ServerURL}/game/result"; // 서버 URL에 맞게 수정
            string accessToken = Session.SessionManager.Instance.GetAccessToken(); // 액세스 토큰 가져오기
            int retryCount = 0;

            while (retryCount < MaxRetryCount) {
                using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm())) {
                    // 승/패를 서버에 전송하기 위해 JSON 형식으로 데이터 추가
                    string jsonData = JsonConvert.SerializeObject(new { result = isWin }); // result: true(승리) or false(패배)
                    byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);

                    request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success) {
                        // 응답 처리 (게임 결과에 대한 응답을 필요에 따라 처리)
                        string jsonResponse = request.downloadHandler.text;
                        // 서버에서 추가적인 응답이 있다면 처리할 코드 추가
                        yield break;
                    }

                    if (request.responseCode != 401)
                        continue;

                    // 액세스 토큰이 만료되었을 경우, 새로 고침 후 재시도
                    yield return Session.SessionManager.Instance.RefreshAccessTokenRequest((success) => {
                        if (!success) {
                            // 토큰 갱신 실패 시 적절한 처리 필요
                            // 예: 콜백 호출 또는 실패 처리
                        }
                    });
                    retryCount++;
                }
            }

            // 실패 처리 (retryCount 초과 시)
            // 예: 콜백 호출 또는 실패 처리
        }


        // ======================================================
        //                        그 외
        // ======================================================

        // 유저 정보 업데이트
        public class UserDataResponse {
            public string Nickname;
            public int ProfileNum;
            public int Coins;
            public int Grade;
            public int RankPoint;
            public int WinCount;
            public int LoseCount;

            public UserDataResponse(string nickname, int profileNum, int coins, int grade, int rankPoint, int winCount,
                                    int loseCount) {
                this.Nickname = nickname;
                this.ProfileNum = profileNum;
                this.Coins = coins;
                this.Grade = grade;
                this.RankPoint = rankPoint;
                this.WinCount = winCount;
                this.LoseCount = loseCount;
            }
        }
        
        // 단순 응답
        public class CheckResponse {
            public bool success;
            public string message;
        }
        
        // 토큰 응답
        public class TokenResponse {
            public string message;
            public string accessToken;
            public string refreshToken;
            public TokenResponse(string message, string accessToken, string refreshToken) {
                this.message = message;
                this.accessToken = accessToken;
                this.refreshToken = refreshToken;
            }
        }

    }
}
