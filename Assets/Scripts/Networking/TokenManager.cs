using Commons;
using Commons.Models;
using Commons.Patterns;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TokenManager : Singleton<TokenManager> {

    private string _accessToken;
    private string _refreshToken;

    protected override void Awake() {
        base.Awake();
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


}
