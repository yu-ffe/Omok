using Newtonsoft.Json;
using System;
using System.Collections;
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
        //                    유저 정보 가져오기
        // ======================================================

        // ReSharper disable Unity.PerformanceAnalysis
        public IEnumerator GetUserInfoRequest(Action<UserDataResponse> callback) {
            string url = $"{Constants.ServerURL}/user/info";
            string accessToken = Session.SessionManager.Instance.GetAccessToken();
            int retryCount = 0;
            Debug.Log("GetUserInfoRequest: " + accessToken);

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
                    yield return RefreshAccessTokenRequest((success, newAccessToken) => {
                        if (success && !string.IsNullOrEmpty(newAccessToken)) {
                            Session.SessionManager.Instance.UpdateAccessToken(newAccessToken);
                            accessToken = newAccessToken;
                        }
                        else {
                            callback?.Invoke(null);
                        }
                    });
                    retryCount++;
                }
            }
            callback?.Invoke(null);
        }
        // ======================================================
        //                        그 외
        // ======================================================
        
        private static IEnumerator RefreshAccessTokenRequest(Action<bool, string> callback) {
            string url = $"{Constants.ServerURL}/auth/refresh";
            string refreshToken = Session.SessionManager.Instance.GetRefreshToken();

            if (string.IsNullOrEmpty(refreshToken)) {
                callback?.Invoke(false, null);
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm())) {
                request.SetRequestHeader("Authorization", $"Bearer {refreshToken}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = request.downloadHandler.text;
                    var response = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                    if (response != null && !string.IsNullOrEmpty(response.accessToken)) {
                        callback?.Invoke(true, response.accessToken); // 새로운 accessToken 반환
                    }
                    else {
                        callback?.Invoke(false, null);
                    }
                }
                else {
                    callback?.Invoke(false, null);
                    Debug.LogError("RefreshAccessTokenRequest failed: " + request.error);
                }
            }
        }



        // <================ 수정지점 (개인확인용)


        public IEnumerator UserDataRequest() {
            string url = $"{Constants.ServerURL}/userdata"; // 유저 데이터 API 엔드포인트
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var userData = JsonConvert.DeserializeObject<UserDataResponse>(jsonResponse);

                // 유저 데이터 처리 (예: 저장 또는 UI 갱신)
                // PlayerManager.Instance.UpdateUserData(userData);
            }
            else {
                // 서버와 연결 실패 시 처리
                Debug.LogError("UserDataRequest failed: " + request.error);
            }
        }



        // 자동 로그인 시도
        public static IEnumerator TryAutoLogin(System.Action<TokenResponse, UserDataResponse> callback) {
            string url = $"{Constants.ServerURL}/autoLogin"; // 자동 로그인 API 엔드포인트
            string sessionToken = PlayerPrefs.GetString("SessionToken", "");

            if (string.IsNullOrEmpty(sessionToken)) {
                callback(null, null);
                yield break;
            }

            WWWForm form = new WWWForm();
            form.AddField("userId", sessionToken);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<SignInResponse>(jsonResponse);

                // if (response.login.) {
                //     // 자동 로그인 성공 시 콜백 호출
                //     callback(response.login, response.data);
                // }
                // else {
                //     // 자동 로그인 실패 시 콜백 호출
                //     callback(null, null);
                // }
            }
            else {
                callback(null, null);
            }
        }

        // UserData 저장 요청 함수
        public IEnumerator SaveNewUserDataRequest(Action<bool, string> callback) {
            string url = $"{Constants.ServerURL}saveUserData"; // UserData 저장 API 엔드포인트
            string token = Session.SessionManager.Instance.GetAccessToken();

            // 세션 검증을 위한 헤더 추가
            if (string.IsNullOrEmpty(token)) {
                callback(false, "세션 토큰이 유효하지 않습니다.");
                yield break;
            }

            PlayerData playerData = PlayerManager.Instance.playerData;

            WWWForm form = new WWWForm();
            // TODO: 기본값으로 모두 변경
            form.AddField("userId", playerData.id);
            form.AddField("nickname", playerData.nickname);
            form.AddField("password", playerData.password);
            form.AddField("profileNum", playerData.profileNum);
            form.AddField("coins", playerData.coins);
            form.AddField("grade", playerData.grade);
            form.AddField("rankPoint", playerData.rankPoint);
            form.AddField("winCount", playerData.winCount);
            form.AddField("loseCount", playerData.loseCount);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("Authorization", "Bearer " + token); // 세션 토큰을 헤더에 추가

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<BaseResponse>(jsonResponse);

                if (response.success) {
                    callback(true, response.message);
                }
                else {
                    callback(false, response.message);
                }
            }
            else {
                callback(false, "서버와의 연결이 실패했습니다.");
            }
        }

        // UserData 가져오기 요청 함수
        public static IEnumerator GetUserDataRequest(System.Action<bool, PlayerData, string> callback) {
            string url = $"{Constants.ServerURL}getUserData"; // UserData 가져오기 API 엔드포인트
            WWWForm form = new WWWForm();
            
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                // 서버로부터 응답 받은 JSON 처리
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<GetUserDataResponse>(jsonResponse);

                if (response.success) {
                    // 서버에서 가져온 UserData를 전달
                    PlayerData playerData = new PlayerData(
                        response.nickname,
                        response.profileNum,
                        response.coins,
                        response.grade,
                        response.rankPoint,
                        response.winCount,
                        response.loseCount
                    );

                    callback(true, playerData, response.message);
                }
                else {
                    callback(false, null, response.message);
                }
            }
            else {
                callback(false, null, "서버와의 연결이 실패했습니다.");
            }
        }
    }

    public class SignInResponse {
        public TokenResponse login;
        public UserDataResponse DataResponse;

        public SignInResponse(TokenResponse login, UserDataResponse dataResponse) {
            this.login = login;
            this.DataResponse = dataResponse;
        }
    }



    public class UserDataResponse {
        public string nickname;
        public int profileNum;
        public int coins;
        public int grade;
        public int rankPoint;
        public int winCount;
        public int loseCount;

        public UserDataResponse(string nickname, int profileNum, int coins, int grade, int rankPoint, int winCount,
                                int loseCount) {
            this.nickname = nickname;
            this.profileNum = profileNum;
            this.coins = coins;
            this.grade = grade;
            this.rankPoint = rankPoint;
            this.winCount = winCount;
            this.loseCount = loseCount;
        }
    }


    public class BaseResponse {
        public bool success;
        public string message;
    }

    public class CheckResponse {
        public bool success;
        public string message;
    }


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

    public class GetUserDataResponse : BaseResponse {
        public string nickname;
        public int profileNum;
        public int coins;
        public int grade;
        public int rankPoint;
        public int winCount;
        public int loseCount;
    }
}
