using System.Collections;
using UnityEngine;
using TMPro;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.User {
    public class SignInManager : Singleton<SignInManager> {

        public static void TryLogin(string id, string password, System.Action<bool, string> callback) {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password)) {
                callback(false, "아이디와 비밀번호를 모두 입력하세요.");
                return;
            }

            if (!IsUserExist(id)) {
                callback(false, "존재하지 않는 아이디입니다.");
                return;
            }

            // TODO: 비밀번호 확인 코드 필요 -> 서버에서 처리
            // if (EncryptPassword(password) != savedPassword) {
            //     callback(false, "비밀번호가 일치하지 않습니다.");
            //     return;
            // }

            // 로그인 처리 후 결과를 콜백으로 반환
            SignIn(id, password, callback);
        }

        /// <summary>
        /// 1. 로그인
        /// 2. 세션 재등록 Refresh, Session 토큰 재발급
        /// 3. 데이터 불러오기
        /// </summary>
        static void SignIn(string id, string pwd, System.Action<bool, string> callback) {
            string password = EncryptPassword(pwd);

            PlayerManager.Instance.playerData.SetPrivateData(id, password);

            // 서버에 로그인 요청을 보내는 코루틴 실행
            NetworkManager.Instance.SignInRequest((success, responseMessage) => {
                if (success) {
                    // 로그인 성공 시 세션 갱신을 요청합니다.
                    Debug.Log("로그인 성공");

                    // 세션 갱신 시도
                    IEnumerator refreshSessionCoroutine = Server.Session.SessionManager.Instance.RefreshSession((sessionSuccess) => {
                        if (sessionSuccess) {
                            // 세션 갱신 성공
                            callback(true, "로그인 성공");
                        } else {
                            // 세션 갱신 실패
                            callback(false, "세션 갱신 실패");
                        }
                    });

                    // 세션 갱신 코루틴 실행
                    NetworkManager.Instance.StartCoroutine(refreshSessionCoroutine);
                } else {
                    // 로그인 실패
                    Debug.LogError("로그인 실패: " + responseMessage);
                    // 로그인 실패 콜백 호출
                    callback(false, "로그인 실패: " + responseMessage);
                }
            });
        }

        // ========== 아이디 존재 확인 ========== 
        static bool IsUserExist(string id) {
            // string ids = PlayerPrefs.GetString("user_ids", "");
            // string[] idArray = ids.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            // return System.Array.Exists(idArray, x => x == id);
            return true; // 임시로 true를 반환 (실제 서버와 연동 필요)
        }

        // ========== 비밀번호 암호화 (회원가입과 동일) ========== 
        static string EncryptPassword(string plainPassword) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
