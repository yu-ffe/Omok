using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.User {

    public static class SignUpManager // : MonoBehaviour
    {

        public static void TrySignUp(string id, string password, string passwordCheck, string nickname, int imgIndex, Action<bool, string> callback) {

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordCheck) || string.IsNullOrEmpty(nickname)) {
                callback(false, "모든 항목을 입력하세요.");
                return;
            }

            // TODO: 이메일 -> 아이디 정규화 후 유연하게 사용 ( abcd123@gmail.com === abcd123 )
            if (!IsEmailFormat(id)) {
                callback(false, "올바른 이메일 형식이 아닙니다.");
                return;
            }

            // TODO: 아이디 Valid 확인 -> 서버연동
            // if (IsIdDuplicated(id)) {
            //     callback(false, "이미 존재하는 아이디입니다.");
            //     return;
            // }

            if (password != passwordCheck) {
                callback(false, "비밀번호가 일치하지 않습니다.");
                return;
            }
            
            // 회원가입 처리 후 결과를 반환
            SignUp(id, password, nickname, imgIndex, callback);
            Debug.Log("회원가입 완료");
        }

        /// <summary>
        /// 1. 플레이어 데이터 생성
        /// 2. 회원가입 계정 생성
        /// 3. PlayerData, refresh, session 토큰 발급 및 로컬 저장 -> (id)
        /// 4. refresh 토큰으로 session 토큰 발급
        /// 5. ----
        /// </summary>
        static void SignUp(string id, string pwd, string nickname, int profile, Action<bool, string> callback) {
            string password = EncryptPassword(pwd);

            PlayerManager.Instance.playerData.SetPrivateData(id, nickname, password, profile);
            
            NetworkManager.Instance.SignUpRequest((success, message, refreshToken, sessionToken) => {
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
                    NetworkManager.Instance.SaveUserDataRequest((saveSuccess, saveMessage) => {
                        if (saveSuccess) {
                            Debug.Log("유저 데이터 저장 성공: " + saveMessage);
                        } else {
                            Debug.LogError("유저 데이터 저장 실패: " + saveMessage);
                        }
                    });

                    // 회원가입 성공
                    callback(true, "회원가입 성공");
                } else {
                    Debug.LogError("회원가입 실패: " + message);
                    // 회원가입 실패
                    callback(false, "회원가입 실패: " + message);
                }
            });
        }

        // ========== 이메일 형식 체크 ========== 
        static bool IsEmailFormat(string email) {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
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
