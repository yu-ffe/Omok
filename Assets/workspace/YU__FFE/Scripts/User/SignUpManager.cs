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
    public class SignUpManager : Singleton<SignUpManager> {
        public void TrySignUp(string id, string password, string passwordCheck, string nickname, int imgIndex, Action<bool, string> callback) {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordCheck) || string.IsNullOrEmpty(nickname)) {
                callback(false, "모든 항목을 입력하세요.");
                return;
            }

            // 이메일 형식 확인
            if (!IsEmailFormat(id)) {
                callback(false, "올바른 이메일 형식이 아닙니다.");
                return;
            }

            // 아이디 중복 체크
            CheckIdAvailability(id,
                (idSuccess, idMessage) => {
                    if (!idSuccess) {
                        callback(false, idMessage);
                        return;
                    }

                    // 닉네임 중복 체크
                    CheckNicknameAvailability(nickname,
                        (nicknameSuccess, nicknameMessage) => {
                            if (!nicknameSuccess) {
                                callback(false, nicknameMessage);
                                return;
                            }

                            // 비밀번호 확인
                            if (password != passwordCheck) {
                                callback(false, "비밀번호가 일치하지 않습니다.");
                                return;
                            }

                            // 모든 검증을 마친 후 회원가입 처리
                            SignUp(id, password, nickname, imgIndex, callback);
                        });
                });
        }

        // 아이디 중복 체크
        private void CheckIdAvailability(string id, Action<bool, string> callback) {
            StartCoroutine(NetworkManager.Instance.CheckIdRequest(id,
                (success, message) => {
                    if (success)
                        callback(true, "사용 가능한 아이디입니다.");
                    else
                        callback(false, "이미 존재하는 아이디입니다.");
                }));
        }

        // 닉네임 중복 체크
        private void CheckNicknameAvailability(string nickname, Action<bool, string> callback) {
            StartCoroutine(NetworkManager.Instance.CheckNicknameRequest(nickname,
                (success, message) => {
                    if (success)
                        callback(true, "사용 가능한 닉네임입니다.");
                    else
                        callback(false, "이미 존재하는 닉네임입니다.");
                }));
        }

        /// <summary>
        /// 1. 플레이어 데이터 생성
        /// 2. 회원가입 계정 생성
        /// 3. PlayerData, refresh, session 토큰 발급 및 로컬 저장 -> (id)
        /// 4. refresh 토큰으로 session 토큰 발급
        /// 5. ----
        /// </summary>
        private void SignUp(string id, string pwd, string nickname, int profile, Action<bool, string> callback) {
            string password = EncryptPassword(pwd);

            PlayerManager.Instance.playerData.SetPrivateData(id, nickname, password, profile);

            StartCoroutine(NetworkManager.Instance.SignUpRequest((success, message, refreshToken, sessionToken) => {
                if (success) {
                    Debug.Log("회원가입 성공: " + message);

                    // 리프레시 토큰과 세션 토큰 저장
                    SaveTokens(refreshToken, sessionToken);

                    // 유저 데이터 저장 요청
                    SaveUserData();

                    callback(true, "회원가입 성공");
                }
                else {
                    callback(false, "회원가입 실패: " + message);
                }
                Debug.Log("회원가입 완료");
            }));
        }

        // 리프레시 토큰 및 세션 토큰 저장
        private void SaveTokens(string refreshToken, string sessionToken) {
            if (!string.IsNullOrEmpty(refreshToken)) {
                Server.Session.SessionManager.Instance.SetRefreshToken(refreshToken);
                Debug.Log("리프레시 토큰 저장 완료");
            }

            if (!string.IsNullOrEmpty(sessionToken)) {
                Server.Session.SessionManager.Instance.SetSessionToken(sessionToken);
                Debug.Log("세션 토큰 저장 완료");
            }
        }

        // 유저 데이터 저장 요청
        private void SaveUserData() {
            NetworkManager.Instance.SaveUserDataRequest((saveSuccess, saveMessage) => {
                if (saveSuccess) {
                    Debug.Log("유저 데이터 저장 성공: " + saveMessage);
                }
                else {
                    Debug.LogError("유저 데이터 저장 실패: " + saveMessage);
                }
            });
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
