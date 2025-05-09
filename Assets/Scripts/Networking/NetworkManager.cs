using Commons;
using Commons.Models;
using Commons.Models.Enums;
using Commons.Models.Response;
using Commons.Patterns;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoSingleton<NetworkManager> {

    private const int MaxRetryCount = 3; // 연결 실패시 최대 재시도 횟수

    // ======================================================
    //                        회원가입
    // ======================================================

    // ReSharper disable Unity.PerformanceAnalysis
    public static IEnumerator SignUpRequest(Action<TokenResponse?> callback) {
        string url = $"{Constants.ServerURL}/auth/signup";
        PlayerData playerData = PlayerManager.Instance.playerData;

        WWWForm form = new WWWForm();
        form.AddField("email", playerData.email);
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
    private static IEnumerator CheckDuplicateRequest(string type, string value, Action<ResponseStatus> callback) {
        string url = $"{Constants.ServerURL}/auth/signup/check";
        WWWForm form = new WWWForm();
        form.AddField("type", type); // 'nickname' or 'id' as type
        form.AddField("value", value);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
            yield return request.SendWebRequest();

            callback?.Invoke(JsonConvert.DeserializeObject<ResponseStatus>(request.downloadHandler.text));
        }
    }

    public static IEnumerator CheckIdRequest(string email, Action<ResponseStatus> callback) {
        string url = $"{Constants.ServerURL}/auth/signup/check";
        WWWForm form = new WWWForm();
        form.AddField("type", "email");
        form.AddField("value", email);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
            yield return request.SendWebRequest();
            callback?.Invoke(JsonConvert.DeserializeObject<ResponseStatus>(request.downloadHandler.text));
        }
    }

    public static IEnumerator CheckNicknameRequest(string nickname, Action<ResponseStatus> callback) {
        string url = $"{Constants.ServerURL}/auth/signup/check";
        WWWForm form = new WWWForm();
        form.AddField("type", "nickname");
        form.AddField("value", nickname);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
            yield return request.SendWebRequest();
            callback?.Invoke(JsonConvert.DeserializeObject<ResponseStatus>(request.downloadHandler.text));
        }
    }

    // ======================================================
    //                         로그인
    // ======================================================

    // ReSharper disable Unity.PerformanceAnalysis
    public static IEnumerator SignInRequest(Action<TokenResponse> callback) {
        string url = $"{Constants.ServerURL}/auth/signin";
        PlayerData playerData = PlayerManager.Instance.playerData;

        WWWForm form = new WWWForm();
        form.AddField("email", playerData.email);
        form.AddField("password", playerData.password);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
            yield return request.SendWebRequest();

            string responseText = request.downloadHandler.text;
            callback?.Invoke(JsonConvert.DeserializeObject<TokenResponse>(responseText));
        }
    }

    public static IEnumerator AutoSignInRequest(Action<TokenResponse?> callback) {
        string url = $"{Constants.ServerURL}/auth/signin/autoSignIn";
        string refreshToken = TokenManager.Instance.GetRefreshToken();
        int retryCount = 0;


        Debug.Log("TryAutoSignInRequest");

        while (retryCount < MaxRetryCount) {
            retryCount++;
            Debug.Log(retryCount + "회 재시도");

            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                Debug.Log("TryAutoSignInRequest - UnityWebRequest.Get");
                Debug.Log(refreshToken);
                request.SetRequestHeader("Authorization", $"Bearer {refreshToken}");
                Debug.Log("TryAutoSignInRequest - SetRequestHeader");
                yield return request.SendWebRequest();
                Debug.Log(request.responseCode);
                if (request.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = request.downloadHandler.text;
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
                    callback?.Invoke(tokenResponse);
                    yield break;
                }
            }
            Debug.LogError("GetUserInfoRequest failed");
        }
        callback?.Invoke(null);

    }

    // ======================================================
    //                        로그아웃
    // ======================================================

    public static IEnumerator LogOutRequest(Action<bool> callback) {
        string url = $"{Constants.ServerURL}/logout";
        string refreshToken = TokenManager.Instance.GetRefreshToken();

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
    public static IEnumerator GetUserInfoRequest(Action<PlayerDataResponse?> callback) {
        string url = $"{Constants.ServerURL}/user/info";
        string accessToken = TokenManager.Instance.GetAccessToken();
        int retryCount = 0;

        while (retryCount < MaxRetryCount) {
            retryCount++;
            Debug.Log(retryCount + "회 재시도");
            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = request.downloadHandler.text;
                    PlayerDataResponse playerDataResponse = JsonConvert.DeserializeObject<PlayerDataResponse>(jsonResponse);
                    callback?.Invoke(playerDataResponse);
                    yield break;
                }

                if (request.responseCode != 401)
                    continue;
                // yield return TokenManager.Instance.RefreshAccessTokenRequest((success) => {
                //     if (!success) {
                //         callback?.Invoke(null);
                //         // TODO: 실패 로직 처리 가능성 있음
                //     }
                // });
            }
            Debug.LogError("GetUserInfoRequest failed");
        }
        callback?.Invoke(null);
    }

    public static IEnumerator GetRankingRequest(Action<List<PlayerRanking>> callback) {
        string url = $"{Constants.ServerURL}/user/ranking";

        int retryCount = 0;

        Debug.Log("TryGetRankingRequest");

        while (retryCount < MaxRetryCount) {
            retryCount++;
            Debug.Log($"{retryCount}회 재시도");

            using (UnityWebRequest request = UnityWebRequest.Get(url)) {
                Debug.Log("TryGetRankingRequest - UnityWebRequest.Get");
                yield return request.SendWebRequest();
                Debug.Log($"Response Code: {request.responseCode}");

                if (request.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = request.downloadHandler.text;
                    try {
                        // JSON 응답을 Ranking 배열로 파싱하고, 이를 List로 변환
                        List<PlayerRanking> rankings = JsonConvert.DeserializeObject<PlayerRanking[]>(jsonResponse).ToList();
                        callback?.Invoke(rankings); // 성공적으로 데이터를 받으면 callback 호출
                        yield break; // 성공하면 반복 종료
                    }
                    catch (Exception e) {
                        Debug.LogError($"응답 파싱 실패: {e.Message}");
                    }
                }
                else {
                    Debug.LogError($"요청 실패: {request.error}");
                }
            }

            Debug.LogError("GetRankingRequest 실패");
        }

        callback?.Invoke(null); // 실패하면 null을 반환
    }
// =========================Send=========================

// ReSharper disable Unity.PerformanceAnalysis
    public static IEnumerator SendGameResult(bool isWin) {
        string url = $"{Constants.ServerURL}/game/result"; // 서버 URL에 맞게 수정
        string accessToken = TokenManager.Instance.GetAccessToken(); // 액세스 토큰 가져오기
        int retryCount = 0;

        while (retryCount < MaxRetryCount) {
            retryCount++;
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

            }
        }

        // 실패 처리 (retryCount 초과 시)
        // 예: 콜백 호출 또는 실패 처리
    }


// ======================================================
//                     유저 토큰 검증
// ======================================================

// 일단 사용 X
// private IEnumerator VerifyServerSession(Action<bool> callback) {
//     string url = Constants.ServerURL + "verifySession";
//     WWWForm form = new WWWForm();
//     form.AddField("accessToken", _accessToken);
//
//     UnityWebRequest request = UnityWebRequest.Post(url, form);
//     yield return request.SendWebRequest();
//
//     if (request.result == UnityWebRequest.Result.Success) {
//         var response = JsonUtility.FromJson<CheckResponse>(request.downloadHandler.text);
//         callback(response.Success);
//     }
//     else {
//         Debug.LogError("[SessionManager] 서버와 연결 실패.");
//         callback(false);
//     }
// }
//
// // ======================================================
// //                    유저 토큰 재발행
// // ======================================================
//
// public IEnumerator RefreshAccessTokenRequest(Action<bool> callback) {
//     string url = $"{Constants.ServerURL}/auth/refresh";
//     string refreshToken = GetRefreshToken();
//
//     // TODO: Refresh Token 만료시 로그아웃 처리
//
//     if (string.IsNullOrEmpty(refreshToken)) {
//         callback?.Invoke(false);
//         yield break;
//     }
//
//     using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm())) {
//         request.SetRequestHeader("Authorization", $"Bearer {refreshToken}");
//         yield return request.SendWebRequest();
//
//         if (request.result == UnityWebRequest.Result.Success) {
//             string jsonResponse = request.downloadHandler.text;
//             var response = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
//             if (response != null && !string.IsNullOrEmpty(response.AccessToken)) {
//                 UpdateAccessToken(response.AccessToken);
//                 callback?.Invoke(true); // 새로운 accessToken 반환
//             }
//             else {
//                 callback?.Invoke(false);
//             }
//         }
//         else {
//             callback?.Invoke(false);
//         }
//     }
// }

// 해당 코드는 임시로 동작시킴

    public void GameEndSendForm(GameResult gameReult) {
        StartCoroutine(SendGameReqult(gameReult));
    }

    public IEnumerator SendGameReqult(GameResult gameResult) {
        yield return StartCoroutine(SendGameResult(gameResult == GameResult.Win));
        yield return StartCoroutine(PlayerManager.Instance.UpdateUserData());
    }

    ///////////////////////////////////////////////////// 아래는 코인 차감 로직
    public IEnumerator GameStartRequest(Action<ResponseStatus> callback) {
        string url = $"{Constants.ServerURL}/game/start";

        string accessToken = TokenManager.Instance.GetAccessToken();

        using (UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm())) {
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return request.SendWebRequest();

            string responseText = request.downloadHandler.text;
            callback?.Invoke(JsonConvert.DeserializeObject<ResponseStatus>(responseText));
        }
    }
    
    public IEnumerator GamePurchaseRequest(int num, int price, Action<ResponseStatus> callback) {
        string url = $"{Constants.ServerURL}/game/shop";
        
        string accessToken = TokenManager.Instance.GetAccessToken();

        WWWForm form = new WWWForm();
        form.AddField("num", num);
        form.AddField("price", price);
        
        using (UnityWebRequest request = UnityWebRequest.Post(url, form)) {
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            yield return request.SendWebRequest();

            string responseText = request.downloadHandler.text;
            callback?.Invoke(JsonConvert.DeserializeObject<ResponseStatus>(responseText));
        }
    }

}
