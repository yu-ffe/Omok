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
        /// 1. 로그인
        /// 2. 세션 재등록 Refresh, Session 토큰 재발급
        /// 3. 데이터 불러오기
        /// </summary>
        private void SignIn(string id, string pwd, Action<bool, string> callback) {
            string password = EncryptPassword(pwd);

            // Id, Password 저장 -> 로그인 -> Id, Password 제거
            // 왜 why 굳이 이런방식? -> 이유없음
            PlayerManager.Instance.playerData.SetPrivateData(id, password);

            StartCoroutine(NetworkManager.Instance.SignInRequest((response) => {
                if (response.success) {
                    Debug.Log("로그인 성공");

                    // 로그인 성공 시 세션 갱신
                    UpdateTokens(response.refreshToken, response.accessToken, callback);
                    //TODO: 플레이어 데이터 가져오는 기능 필요.
                    
                } else {
                    // 로그인 실패
                    callback(false, "로그인 실패: " + response.message);
                }
            }));
            
            PlayerManager.Instance.playerData.ClearPrivateData();

        }

        // 세션 갱신 처리
        private void UpdateTokens(string refreshToken, string sessionToken, Action<bool, string> callback) {
            if (!string.IsNullOrEmpty(refreshToken)) {
                Server.Session.SessionManager.Instance.UpdateRefreshToken(refreshToken);
                Debug.Log("리프레시 토큰 저장 완료");
            }

            if (!string.IsNullOrEmpty(sessionToken)) {
                Server.Session.SessionManager.Instance.UpdateSessionToken(sessionToken);
                Debug.Log("세션 토큰 저장 완료");
            }
        }

        // ========== 비밀번호 암호화 (회원가입과 동일) ==========
        private static string EncryptPassword(string plainPassword) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        
        
    }
}
