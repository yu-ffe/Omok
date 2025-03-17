using UnityEngine;
using TMPro;


// 3.14 임원빈
// UI 기능을 따로 분리. MonoBehaviour 불필요. 전역함수로 변경하겠습니다.
// public class SigninManager : MonoBehaviour
public static class SigninManager
{
    // [Header("로그인 UI 연결 (TMP)")]
    // public TMP_InputField inputId;
    // public TMP_InputField inputPassword;

    // [Header("알림 텍스트 (TMP)")]
    // public TMP_Text alertText;


    //3.14 임원빈
    // 로그인 호출 함수 추가 리턴값 (결과 / 메세지)
    // 기존 함수는 혹시 모르니 주석처리하겠습니다.
    public static (bool isSuccsess, string message) TryLogin(string id, string password)
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            return (false, "아이디와 비밀번호를 모두 입력하세요.");
        }

        if (!IsUserExist(id))
        {
            return (false, "존재하지 않는 아이디입니다.");
        }

        string savedPassword = PlayerPrefs.GetString($"user_{id}_password");
        if (EncryptPassword(password) != savedPassword)
        {
            return (false, "비밀번호가 일치하지 않습니다.");
        }



        return SessionUp(id);       
    }

    

    public static (bool isSuccsess, string message) SessionUp(string id)
    {
        // ========= 세션 등록 (전체 데이터 포함) =========
        string nickname = PlayerPrefs.GetString($"user_{id}_nickname");
        int profileNum = PlayerPrefs.GetInt($"user_{id}_profile");
        int coins = PlayerPrefs.GetInt($"user_{id}_coins", 0);
        int grade = PlayerPrefs.GetInt($"user_{id}_grade", 18); // 기본 18급
        int rankPoint = PlayerPrefs.GetInt($"user_{id}_rankPoint", 0); // 기본 0
        int winCount = PlayerPrefs.GetInt($"user_{id}_winCount", 0); // 기본 0
        int loseCount = PlayerPrefs.GetInt($"user_{id}_loseCount", 0); // 기본 0

        SessionManager.AddSession(id, nickname, profileNum, coins, grade,
            rankPoint, winCount, loseCount);
        SessionManager.currentUserId = id; // 현재 유저 ID 저장

        // ========= 로그인 성공 알림 =========
        Debug.Log($"로그인 성공! {nickname}님 환영합니다. (급수: {grade}급, 포인트: {rankPoint})");

        // 로그인 성공 시 자동 로그인 정보 저장
        AutoLogin.LastLoginUserSave(id);

        // alertText.text = $"{nickname}님 환영합니다!";
        return (true, $"{nickname}님 환영합니다!");
        // 메인 화면 이동
        // SceneManager.LoadScene("MainScene"); 
    }


    //3.14 임원빈
    // 관련 함수들 전역함수로 변경
    // public bool IsUserExist(string id) -> public static bool IsUserExist(string id)
    // public string EncryptPassword(string plainPassword) -> public static string EncryptPassword(string plainPassword)
    // OnClickGoToSignUp 삭제


    // ========== 로그인 버튼 클릭 시 호출 ==========
    // public void OnClickLogin()
    // {
    //     string id = inputId.text.Trim();
    //     string password = inputPassword.text;

    //     if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
    //     {
    //         // alertText.text = "아이디와 비밀번호를 모두 입력하세요.";
    //         return;
    //     }

    //     if (!IsUserExist(id))
    //     {
    //         // alertText.text = "존재하지 않는 아이디입니다.";
    //         return;
    //     }

    //     string savedPassword = PlayerPrefs.GetString($"user_{id}_password");
    //     if (EncryptPassword(password) != savedPassword)
    //     {
    //         // alertText.text = "비밀번호가 일치하지 않습니다.";
    //         return;
    //     }

    //     // ========= 세션 등록 (전체 데이터 포함) =========
    //     string nickname = PlayerPrefs.GetString($"user_{id}_nickname");
    //     int profileNum = PlayerPrefs.GetInt($"user_{id}_profile");
    //     int coins = PlayerPrefs.GetInt($"user_{id}_coins", 0);
    //     int grade = PlayerPrefs.GetInt($"user_{id}_grade", 18); // 기본 18급
    //     int rankPoint = PlayerPrefs.GetInt($"user_{id}_rankPoint", 0); // 기본 0
    //     int winCount = PlayerPrefs.GetInt($"user_{id}_winCount", 0); // 기본 0
    //     int loseCount = PlayerPrefs.GetInt($"user_{id}_loseCount", 0); // 기본 0

    //     SessionManager.AddSession(id, nickname, profileNum, coins, grade,
    //         rankPoint, winCount, loseCount);
    //     SessionManager.currentUserId = id; // 현재 유저 ID 저장

    //     // ========= 로그인 성공 알림 =========
    //     Debug.Log($"로그인 성공! {nickname}님 환영합니다. (급수: {grade}급, 포인트: {rankPoint})");
    //     // alertText.text = $"{nickname}님 환영합니다!";

    //     // 메인 화면 이동
    //     // SceneManager.LoadScene("MainScene"); 
    // }

    // ========== 아이디 존재 확인 ==========
    static bool IsUserExist(string id)
    {
        string ids = PlayerPrefs.GetString("user_ids", "");
        string[] idArray = ids.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        return System.Array.Exists(idArray, x => x == id);
    }

    // ========== 비밀번호 암호화 (회원가입과 동일) ==========
    static string EncryptPassword(string plainPassword)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    // ========== 회원가입 화면으로 이동 ==========
    // static public void OnClickGoToSignUp()
    // {
    //     // SceneManager.LoadScene("SignUpScene");
    //     Debug.Log("회원가입 화면으로 이동");
    // }
}
