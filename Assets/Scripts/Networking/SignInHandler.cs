using Commons;
using Commons.Models;
using Commons.Models.Response;
using Commons.Patterns;
using Commons.Utils;
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class SignInHandler : MonoSingleton<SignInHandler> {
    private const string AutoLoginEnabled = "AutoLoginEnabledKey";

    public void AttemptSignIn(string email, string password, Action<bool, string> callback) {
        
        if (!ValidationManager.Validate("email", email, callback)) return;
        if (!ValidationManager.Validate("password", password, callback)) return;

        UtilityManager.EncryptPassword(ref password);

        StartCoroutine(SignIn(email, password, callback));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static IEnumerator SignIn(string email, string password, Action<bool, string> callback) {

        PlayerManager.Instance.playerData.SetPrivateData(email, password);

        TokenResponse? tokenResponse = null;
        yield return NetworkManager.SignInRequest(response => {
            tokenResponse = response;
            PlayerManager.Instance.playerData.ClearPrivateData();
        });
        if (tokenResponse is null) {
            callback(false, "로그인 실패");
            yield break;
        }
        
        TokenManager.Instance.UpdateTokens(tokenResponse.Value.RefreshToken, tokenResponse.Value.AccessToken);

        yield return LoadPlayerData(callback);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator AttemptAutoSignIn(Action<bool, string> callback) {

        Debug.Log("[SignInHandler] AttemptAutoSignIn");
        if (!GetAutoLoginEnabled()) {
            callback(false, "자동 로그인 비활성화");
            yield break;
        }
        
        if (TokenManager.Instance.GetRefreshToken() is null || TokenManager.Instance.GetRefreshToken() == "") {
            callback(false, "자동 로그인 실패: 리프레시 토큰 없음");
            yield break;
        }
        
        Debug.Log("[SignInHandler] AttemptAutoSignIn - TryAutoSignInRequest");
        TokenResponse? tokenResponse = null;
        StartCoroutine(NetworkManager.AutoSignInRequest(response => {
            Debug.Log("[SignInHandler] AttemptAutoSignIn - TryAutoSignInRequest - Response");
            Debug.Log(response);
            tokenResponse = response;
            
            Debug.Log("[SignInHandler] AttemptAutoSignIn - TokenResponse");
            if (tokenResponse is null) {
                callback(false, "자동 로그인 실패");
                Debug.Log("[SignInHandler] AttemptAutoSignIn - TokenResponse is null");
            }
            Debug.Log("[SignInHandler] AttemptAutoSignIn - UpdateTokens");
            TokenManager.Instance.UpdateTokens(tokenResponse.Value.RefreshToken, tokenResponse.Value.AccessToken);
            callback(true, "자동 로그인 성공");

            Debug   .Log("[SignInHandler] AttemptAutoSignIn - LoadPlayerData");
            StartCoroutine(LoadPlayerData(callback));
        }));
    }

    private static IEnumerator LoadPlayerData(Action<bool, string> callback) {
        PlayerDataResponse? playerInfoResponse = null;
        yield return NetworkManager.GetUserInfoRequest(response => { playerInfoResponse = response; });
        if (playerInfoResponse is null) {
            callback(false, "사용자 정보 요청 실패");
            yield break;
        }

        PlayerManager.Instance.SetPlayerData(playerInfoResponse.Value);
        callback(true, "로그인 성공");
    }

    public static bool GetAutoLoginEnabled() {
        return PlayerPrefs.GetInt(AutoLoginEnabled, 1) == 1;
    }

    public void SetAutoLoginEnabled(bool isEnabled) {
        PlayerPrefs.SetInt(AutoLoginEnabled, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

}
