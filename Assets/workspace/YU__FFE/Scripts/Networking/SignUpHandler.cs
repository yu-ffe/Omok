using Commons.Models;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using workspace.YU__FFE.Scripts.Commons;

namespace workspace.YU__FFE.Scripts.Networking {

    public class SignUpHandler : Singleton<SignUpHandler> {

        public void AttemptSignUp(string id, string password, string passwordCheck, string nickname, int imgIndex, Action<bool, string> callback) {

            if (!ValidationManager.Validate("email", id, callback)) return;
            if (!ValidationManager.Validate("password", password, callback)) return;
            if (!ValidationManager.ValidatePasswordMatch(password, passwordCheck, callback)) return;
            if (!ValidationManager.Validate("nickname", nickname, callback)) return;

            UtilityManager.EncryptPassword(ref password);

            StartCoroutine(CheckAndSignUp(id, password, nickname, imgIndex, callback));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator CheckAndSignUp(string id, string pwd, string nickname, int profile, Action<bool, string> callback) {

            CheckResponse checkResponse = null;
            yield return NetworkManager.CheckIdRequest(id,
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

            yield return SignUp(id, pwd, nickname, profile, callback);
        }

        private static IEnumerator SignUp(string id, string password, string nickname, int profile, Action<bool, string> callback) {

            PlayerManager.Instance.playerData.SetPrivateData(id, nickname, password, profile);

            TokenResponse tokenResponse = null;
            yield return NetworkManager.SignUpRequest((response) => {
                PlayerManager.Instance.playerData.ClearPrivateData();
                tokenResponse = response;
            });
            if (tokenResponse is null) {
                callback(false, "회원가입에 실패했습니다.");
                yield break;
            }

            workspace.YU__FFE.Scripts.Networking.TokenManager.Instance.UpdateTokens(tokenResponse.RefreshToken, tokenResponse.AccessToken);
            callback(tokenResponse != null, tokenResponse?.Message);

        }

    }

}
