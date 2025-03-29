using Commons.Models;
using Commons.Patterns;
using Commons.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;

public class SignUpHandler : Singleton<SignUpHandler> {

    public void AttemptSignUp(string email, string password, string passwordCheck, string nickname, int imgIndex, Action<bool, string> callback) {

        if (!ValidationManager.Validate("email", email, callback)) return;
        if (!ValidationManager.Validate("password", password, callback)) return;
        if (!ValidationManager.ValidatePasswordMatch(password, passwordCheck, callback)) return;
        if (!ValidationManager.Validate("nickname", nickname, callback)) return;
        if(!ValidationManager.Validate("imgIndex", imgIndex, callback)) return;

        UtilityManager.EncryptPassword(ref password);

        StartCoroutine(CheckAndSignUp(email, password, nickname, imgIndex, callback));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator CheckAndSignUp(string email, string pwd, string nickname, int profile, Action<bool, string> callback) {

        CheckResponse checkResponse = null;
        yield return NetworkManager.CheckIdRequest(email,
            (response) => { checkResponse = response; });
        if (checkResponse is null) {
            callback(false, "서버와의 통신에 실패했습니다.");
            yield break;
        }
        else if (!checkResponse.Success) {
            callback(false, checkResponse.Message);
            yield break;
        }

        yield return NetworkManager.CheckNicknameRequest(nickname,
            (response) => { checkResponse = response; });
        if (checkResponse is null) {
            callback(false, "서버와의 통신에 실패했습니다.");
            yield break;
        }
        else if (!checkResponse.Success) {
            callback(false, checkResponse.Message);
            yield break;
        }

        yield return SignUp(email, pwd, nickname, profile, callback);
    }

    private static IEnumerator SignUp(string email, string password, string nickname, int profile, Action<bool, string> callback) {

        PlayerManager.Instance.playerData.SetPrivateData(email, nickname, password, profile);

        TokenResponse tokenResponse = null;
        yield return NetworkManager.SignUpRequest((response) => {
            PlayerManager.Instance.playerData.ClearPrivateData();
            tokenResponse = response;
        });
        if (tokenResponse is null) {
            callback(false, "회원가입에 실패했습니다.");
            yield break;
        }

        callback(tokenResponse != null, tokenResponse?.Message);
        
    }

}
