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
    public class SignUpHandler : Singleton<SignUpHandler> {

        public void TrySignUp(string id, string password, string passwordCheck, string nickname, int imgIndex, Action<bool, string> callback) {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordCheck) || string.IsNullOrEmpty(nickname)) {
                callback(false, "모든 항목을 입력하세요.");
                return;
            }

            // 이메일 형식 제거 -> id 변수값에 맞춰서 다 만들어서 변수명 다 바꾸기 어려움 (서버포함)
            // 나중에 가능할때 다시 수정
            // 이메일 형식 확인
            // if (!IsEmailFormat(id)) {
            //     callback(false, "올바른 이메일 형식이 아닙니다.");
            //     return;
            // }

            //이거는 테스트 후 사용
            // if (!IsPasswordValid(password)) {
            //     callback(false, "비밀번호는 8자 이상, 영문, 숫자, 특수문자를 포함해야 합니다.");
            //     return;
            // }

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
                (checkResponse) => { callback(checkResponse.success, checkResponse.message); }));
        }

        private void CheckNicknameAvailability(string nickname, Action<bool, string> callback) {
            StartCoroutine(NetworkManager.CheckNicknameRequest(nickname,
                (checkResponse) => { callback(checkResponse.success, checkResponse.message); }));
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
                    Server.Session.SessionManager.Instance.UpdateTokens(response.refreshToken, response.accessToken);
                    SaveNewUserData();
                }
                callback(response != null, response?.message);
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
