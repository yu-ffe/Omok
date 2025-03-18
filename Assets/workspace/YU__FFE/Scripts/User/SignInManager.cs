using System;
using System.Collections;
using UnityEngine;
using TMPro;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.User {
    public class SignInManager : Singleton<SignInManager> {

        public void TrySignIn(string id, string password, Action<bool, string> callback) {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password)) {
                callback(false, "아이디와 비밀번호를 모두 입력하세요.");
                return;
            }

            // 로그인 처리
            SignIn(id, password, callback);
        }

        /// <summary>
        /// 1. 로그인 시도 및 토큰 갱신
        /// 2. 세션 토큰으로 UserData 불러오기 및 저장
        /// </summary>
        private void SignIn(string id, string pwd, Action<bool, string> callback) {
            string password = EncryptPassword(pwd);

            PlayerManager.Instance.playerData.SetPrivateData(id, password);

            StartCoroutine(NetworkManager.SignInRequest((response) => {
                if (response is not null) {
                    Server.Session.SessionManager.Instance.UpdateTokens(response.refreshToken, response.accessToken);
                    NetworkManager.Instance.GetUserInfoRequest((data) => {
                        if (data is not null) {
                            UpdateUserData(data);
                        }
                        // 이부분은 따로 로그 찍히게 수정
                    });
                } 
                callback(response != null, response?.message);
            }));
            
            PlayerManager.Instance.playerData.ClearPrivateData();

        }

        private void UpdateUserData(UserDataResponse dataResponse) {
            PlayerManager.Instance.playerData.nickname = dataResponse.nickname;
            PlayerManager.Instance.playerData.profileNum = dataResponse.profileNum;
            PlayerManager.Instance.playerData.coins = dataResponse.coins;
            PlayerManager.Instance.playerData.grade = dataResponse.grade;
            PlayerManager.Instance.playerData.rankPoint = dataResponse.rankPoint;
            PlayerManager.Instance.playerData.winCount = dataResponse.winCount;
            PlayerManager.Instance.playerData.loseCount = dataResponse.loseCount;
        }

        // ========== 비밀번호 암호화 (회원가입과 동일) ==========
        private static string EncryptPassword(string plainPassword) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        
        
    }
}
