using UnityEngine;
using TMPro;

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

        // ========= 세션 등록 (전체 데이터 포함) =========
        string nickname = PlayerPrefs.GetString($"user_{id}_nickname");
        int profileNum = PlayerPrefs.GetInt($"user_{id}_profile");
        int coins = PlayerPrefs.GetInt($"user_{id}_coins", 0);
        int grade = PlayerPrefs.GetInt($"user_{id}_grade", 18); // 기본 18급
        int rankPoint = PlayerPrefs.GetInt($"user_{id}_rankPoint", 0); // 기본 0
        int winCount = PlayerPrefs.GetInt($"user_{id}_winCount", 0); // 기본 0
        int loseCount = PlayerPrefs.GetInt($"user_{id}_loseCount", 0); // 기본 0

        SessionManager.AddSession(id,
            nickname,
            profileNum,
            coins,
            grade,
            rankPoint,
            winCount,
            loseCount);
        SessionManager.currentUserId = id; // 현재 유저 ID 저장

        // ========= 로그인 성공 알림 =========
        Debug.Log($"로그인 성공! {nickname}님 환영합니다. (급수: {grade}급, 포인트: {rankPoint})");
        alertText.text = $"{nickname}님 환영합니다!";

        // 메인 화면 이동
        // SceneManager.LoadScene("MainScene"); 
    }

    // ========== 아이디 존재 확인 ==========
    bool IsUserExist(string id)
    {
        string ids = PlayerPrefs.GetString("user_ids", "");
        string[] idArray = ids.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
        return System.Array.Exists(idArray, x => x == id);
    }

    // ========== 비밀번호 암호화 (회원가입과 동일) ==========
    string EncryptPassword(string plainPassword)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPassword);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    // ========== 회원가입 화면으로 이동 ==========
    public void OnClickGoToSignUp()
    {
        // SceneManager.LoadScene("SignUpScene");
        Debug.Log("회원가입 화면으로 이동");
    }
}
