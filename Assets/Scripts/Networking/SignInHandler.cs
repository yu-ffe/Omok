using Commons;
using Commons.Models;
using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class SignInHandler : Singleton<SignInHandler> {
    private const string AutoLoginEnabledKey = "AutoLoginEnabledKey";

    public (bool, string) TrySignIn(string id, string password) {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password)) {
            return (false, "아이디와 비밀번호를 모두 입력하세요.");
        }

        bool isSuccess = false;
        string message = "로그인 실패";

        SignIn(id, password, (b, s) => {
            isSuccess = b;
            message = s;
        });

        return (isSuccess, message);
    }

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
                SessionManager.Instance.UpdateTokens(response.RefreshToken, response.AccessToken);
                NetworkManager.GetUserInfoRequest(data => {
                    if (data != null) {
                        UpdateUserData(data);
                    }
                });
            }
            callback(response != null, response?.Message);
        }));

        PlayerManager.Instance.playerData.ClearPrivateData();
    }

    private void UpdateUserData(PlayerDataResponse dataResponse) {
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

    public IEnumerator AttemptAutoLogin(Action<bool, string> callback) {
        if (!GetAutoLoginEnabled()) {
            Debug.Log("자동 로그인 비활성화됨");
            UI_Manager.Instance.Show(UI_Manager.PanelType.Login);
            callback(false, "자동 로그인 비활성화");
            yield break;
        }
        yield return RefreshSessionForAutoLogin(callback);
    }

    private IEnumerator RefreshSessionForAutoLogin(Action<bool, string> callback) {
        // 현재 저장된 refreshToken 가져오기
        string refreshToken = SessionManager.Instance.GetRefreshToken();

        if (!string.IsNullOrEmpty(refreshToken)) {
            // 토큰을 이용하여 자동 로그인 시도
            yield return StartCoroutine(NetworkManager.TryAutoLoginRequest(success => {
                if (success is not null) {
                    // 토큰 갱신 성공시 사용자 정보를 요청
                    NetworkManager.TryAutoLoginRequest(data => {
                        if (data is not null) {
                            // 사용자 데이터 업데이트
                            SessionManager.Instance.UpdateTokens(success.RefreshToken, success.AccessToken);
                            // TODO: 자동로그인하면 서버에서 데이터 가져오는 로직도 추가
                            callback?.Invoke(true, "자동 로그인 성공");
                        }
                        else {
                            callback?.Invoke(false, "사용자 정보 요청 실패");
                        }
                    });
                }
                else {
                    callback?.Invoke(false, "토큰 갱신 실패");
                }
            }));
        } else {
            callback?.Invoke(false, "로그인 정보 없음");
        }
    }

    public static bool GetAutoLoginEnabled() {
        return PlayerPrefs.GetInt(AutoLoginEnabledKey, 1) == 1;
    }

    public void SetAutoLoginEnabled(bool isEnabled) {
        PlayerPrefs.SetInt(AutoLoginEnabledKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
