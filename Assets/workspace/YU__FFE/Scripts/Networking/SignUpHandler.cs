using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using workspace.YU__FFE.Scripts.Commons;

namespace YU_FFEE {
    public class SignUpHandler : Singleton<SignUpHandler> {

        public void AttemptSignUp(string id, string password, string passwordCheck, string nickname, int imgIndex, Action<bool, string> callback) {
           
            if (!ValidationManager.Validate("email", id, callback)) return;
            if (!ValidationManager.Validate("password", password, callback)) return;
            if(!ValidationManager.ValidatePasswordMatch(password, passwordCheck, callback)) return;
            if (!ValidationManager.Validate("nickname", nickname, callback)) return;
            
            UtilityManager.EncryptPassword(ref password);

            if (!CheckPasswordMatch(password, passwordCheck)) {
                callback(false, "비밀번호가 일치하지 않습니다.");
                return;
            }

            Debug.Log("아이디 중복 확인");
            CheckIdAvailability(id,
                (idSuccess, idMessage) => {
                    if (!idSuccess) {
                        callback(false, idMessage);
                        return;
                    }

                    CheckNicknameAvailability(nickname,
                        (nicknameSuccess, nicknameMessage) => {
                            if (!nicknameSuccess) {
                                callback(false, nicknameMessage);
                                return;
                            }

                            SignUp(id, password, nickname, imgIndex, callback);
                        });
                });
        }

        private static bool CheckPasswordMatch(string password, string passwordCheck) {
            return password == passwordCheck;
        }

        private static bool IsPasswordValid(string password) {
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private void CheckIdAvailability(string id, Action<bool, string> callback) {
            StartCoroutine(NetworkManager.CheckIdRequest(id,
                (checkResponse) => { callback(checkResponse.Success, checkResponse.Message); }));
        }

        private void CheckNicknameAvailability(string nickname, Action<bool, string> callback) {
            StartCoroutine(NetworkManager.CheckNicknameRequest(nickname,
                (checkResponse) => { callback(checkResponse.Success, checkResponse.Message); }));
        }

        /// <summary>
        /// 1. PlayerData 저장
        /// 2. 회원가입 시도 + 토큰 발행 및 저장
        /// 3. PlayerData 개인정보 해제
        /// </summary>
        private void SignUp(string id, string pwd, string nickname, int profile, Action<bool, string> callback) {
            string password = EncryptPassword(pwd);

            PlayerManager.Instance.playerData.SetPrivateData(id, nickname, password, profile);

            StartCoroutine(NetworkManager.SignUpRequest((response) => {
                if (response is not null) {
                    // TODO: Auto SignIn하고 아래 코드 실행
                    workspace.YU__FFE.Scripts.Networking.SessionManager.Instance.UpdateTokens(response.RefreshToken, response.AccessToken);
                    SaveNewUserData();
                }
                callback(response != null, response?.Message);
            }));

            PlayerManager.Instance.playerData.ClearPrivateData();
        }

// 유저 데이터 저장 요청
        private void SaveNewUserData() {
            NetworkManager.GetUserInfoRequest((response) => { });
        }

// 이메일 형식 체크
        private static bool IsEmailFormat(string email) {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

// 비밀번호 암호화 (Base64)
        private static string EncryptPassword(string plainPassword) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}