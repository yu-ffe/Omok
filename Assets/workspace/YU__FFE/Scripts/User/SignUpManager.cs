using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.User {

    public static class SignUpManager // : MonoBehaviour
    {

        public static (bool isSuccsess, string message) TrySignUp(string id, string password, string passwordCheck, string nickname, int imgIndex) {

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordCheck) || string.IsNullOrEmpty(nickname)) {
                return (false, "모든 항목을 입력하세요.");
            }

            // TODO: 이메일 -> 아이디 정규화 후 유연하게 사용 ( abcd123@gmail.com === abcd123 )
            if (!IsEmailFormat(id)) {
                return (false, "올바른 이메일 형식이 아닙니다.");
            }

            // TODO: 아이디 Valid 확인 -> 서버연동
            // if (IsIdDuplicated(id)) {
            //     return (false, "이미 존재하는 아이디입니다.");
            // }

            if (password != passwordCheck) {
                return (false, "비밀번호가 일치하지 않습니다.");
            }
            
            SaveUserData(id, password, nickname, imgIndex);
            Debug.Log("회원가입 완료");
            AutoLogin.AttemptAutoLogin(id);
            return (true, "회원 가입 성공");
        }

        /// <summary>
        /// 1. 플레이어 데이터 생성
        /// 2. 회원가입 시도
        /// 3. PlayerData, refresh, session 토큰 발급 및 로컬 저장 -> (id)
        /// 4. refresh 토큰으로 session 토큰 발급
        /// 5. ----
        /// </summary>
        static void SaveUserData(string id, string pwd, string nickname, int profile) {
            string password = EncryptPassword(pwd);

            // 기본 값
            int coins = 1000;
            int grade = 18; // 시작 급수
            int rankPoint = 0; // 시작 포인트
            int winCount = 0; // 시작 승리 수
            int loseCount = 0; // 시작 패배 수

            PlayerManager.Instance.playerData = new PlayerData(id, nickname, password, profile, coins, grade, rankPoint, winCount, loseCount);
            IEnumerator signUpRequest = NetworkManager.Instance.SignUpRequest((success, message, refreshToken, sessionToken) => {
                if (success) {
                    Debug.Log("회원가입 성공: " + message);

                    // 리프레시 토큰과 세션 토큰 저장
                    if (!string.IsNullOrEmpty(refreshToken)) {
                        // 자동 로컬 저장
                        Server.Session.SessionManager.Instance.SetRefreshToken(refreshToken);
                        Debug.Log("리프레시 토큰 저장 완료");
                    }

                    if (!string.IsNullOrEmpty(sessionToken)) {
                        Server.Session.SessionManager.Instance.SetSessionToken(sessionToken);
                        Debug.Log("세션 토큰 저장 완료");
                    }

                    // 유저 데이터 저장 요청
                    NetworkManager.Instance.SaveUserDataRequest(PlayerManager.Instance.playerData, (saveSuccess, saveMessage) => {
                        if (saveSuccess) {
                            Debug.Log("User data saved successfully: " + saveMessage);
                        } else {
                            Debug.LogError("Failed to save user data: " + saveMessage);
                        }
                    });
                } else {
                    Debug.LogError("회원가입 실패: " + message);
                }
            });
        }


        // ========== 이메일 형식 체크 ==========
        static bool IsEmailFormat(string email) {
            return email.Contains("@") && email.Contains(".");
        }

        // ========== 아이디 중복 확인 ==========
        // static bool IsIdDuplicated(string id) {
        //     string ids = PlayerPrefs.GetString("user_ids", "");
        //     string[] idArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //     return idArray.Contains(id);
        // }

        // ========== 비밀번호 암호화 (Base64) ==========
        static string EncryptPassword(string plainPassword) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }

}
