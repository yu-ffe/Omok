using UnityEngine;
using TMPro;

namespace MJ
{
    public class SigninManager : MonoBehaviour
    {
        [Header("로그인 UI 연결 (TMP)")]
        public TMP_InputField inputId;
        public TMP_InputField inputPassword;

        [Header("알림 텍스트 (TMP)")]
        public TMP_Text alertText;

        // ========== 로그인 버튼 클릭 시 호출 ==========
        public void OnClickLogin()
        {
            string id = inputId.text.Trim();
            string password = inputPassword.text;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                alertText.text = "아이디와 비밀번호를 모두 입력하세요.";
                return;
            }

            if (!IsUserExist(id))
            {
                alertText.text = "존재하지 않는 아이디입니다.";
                return;
            }

            string savedPassword = PlayerPrefs.GetString($"user_{id}_password");
            if (EncryptPassword(password) != savedPassword)
            {
                alertText.text = "비밀번호가 일치하지 않습니다.";
                return;
            }

            // 로그인 성공 시 세션 저장
            SessionManager.currentUserId = id;
            SessionManager.nickname = PlayerPrefs.GetString($"user_{id}_nickname");
            SessionManager.profileNum = PlayerPrefs.GetInt($"user_{id}_profile");

            Debug.Log($"로그인 성공! {SessionManager.nickname}님 환영합니다.");
            alertText.text = $"{SessionManager.nickname}님 환영합니다!";

            // (선택) 메인 화면 이동
            // SceneManager.LoadScene("MainScene"); // 예시
        }

        // ========== 아이디 존재 확인 ==========
        bool IsUserExist(string id)
        {
            string ids = PlayerPrefs.GetString("user_ids", "");
            string[] idArray = ids.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            return System.Array.Exists(idArray, x => x == id);
        }

        // ========== 비밀번호 암호화 (회원가입과 동일한 방식) ==========
        string EncryptPassword(string plainPassword)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        // ========== (선택) 회원가입 화면으로 이동 ==========
        public void OnClickGoToSignUp()
        {
            // SceneManager.LoadScene("SignUpScene"); // 실제 사용 예
            Debug.Log("회원가입 화면으로 이동");
        }
    }

    // ========== 현재 로그인 유저 저장용 클래스 ==========
    public static class SessionManager
    {
        public static string currentUserId;
        public static string nickname;
        public static int profileNum;
    }
}
