using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SessionManager
{
    // 현재 게임 세션 내 모든 유저 정보 (메모리 저장)
    private static Dictionary<string, UserSession> userSessions = new Dictionary<string, UserSession>();

    public static string currentUserId; // 현재 로그인 유저 ID
    public static Sprite[] ProfileSprites; // 프로필 이미지 스프라이트 목록(외부에서 게임이 시작될때, 초기화 필요)

    [System.Serializable]
    public class UserSession
    {
        public string Nickname;
        public int ProfileNum;
        public int Coins;
        public int Grade;
        public int RankPoint;
        public int WinCount;  // 승리 횟수
        public int LoseCount; // 패배 횟수

        // 생성자
        public UserSession(string nickname, int profileNum, int coins, int grade,
            int rankPoint, int winCount, int loseCount)
        {
            Nickname = nickname;
            ProfileNum = profileNum;
            Coins = coins;
            Grade = grade;
            RankPoint = rankPoint;
            WinCount = winCount;
            LoseCount = loseCount;
        }
    }

    // ========== 세션 추가 및 저장 ==========
    public static void AddSession(string userId, string nickname, int profileNum,
        int coins, int grade, int rankPoint, int winCount, int loseCount)
    {
        if (!userSessions.ContainsKey(userId))
        {
            userSessions[userId] = new UserSession(nickname, profileNum, coins, grade,
                rankPoint, winCount, loseCount);
            Debug.Log($"세션 추가: {userId} - {nickname}");
        }
        else
        {
            Debug.Log($"이미 존재하는 유저: {userId}, 세션 갱신");
            userSessions[userId] = new UserSession(nickname, profileNum, coins, grade,
                rankPoint, winCount, loseCount);
        }

        SaveUserSession(userId); // 저장
    }

    // ========== 특정 유저 세션 가져오기 ==========
    public static UserSession GetSession(string userId)
    {
        return userSessions.ContainsKey(userId) ? userSessions[userId] : null;
    }

    // ========== 모든 유저 세션 불러오기 (게임 시작 시) ==========
    public static void LoadAllSessions()
    {
        userSessions.Clear();
        foreach (string userId in GetAllStoredUserIds())
        {
            string json = PlayerPrefs.GetString(userId, "");
            if (!string.IsNullOrEmpty(json))
            {
                UserSession session = JsonUtility.FromJson<UserSession>(json);
                userSessions[userId] = session;
                Debug.Log($"세션 로드: {userId} - {session.Nickname}");
            }
        }
        Debug.Log($"총 {userSessions.Count}명의 유저 세션 로드 완료");
    }

    // ========== 특정 유저 세션 저장 (Json 방식) ==========
    private static void SaveUserSession(string userId)
    {
        if (userSessions.ContainsKey(userId))
        {
            string json = JsonUtility.ToJson(userSessions[userId]);
            PlayerPrefs.SetString(userId, json);
            SaveUserId(userId); // ID 리스트 갱신
            PlayerPrefs.Save();
            Debug.Log($"세션 저장 완료: {userId}");
        }
    }

    // ========== 특정 유저 세션 삭제 ==========
    public static void RemoveSession(string userId)
    {
        if (userSessions.ContainsKey(userId))
        {
            userSessions.Remove(userId);
            PlayerPrefs.DeleteKey(userId);
            RemoveUserId(userId);
            PlayerPrefs.Save();
            Debug.Log($"세션 삭제: {userId}");
        }
    }

    // ========== 저장된 모든 유저 ID 리스트 가져오기 ==========
    private static List<string> GetAllStoredUserIds()
    {
        string storedIds = PlayerPrefs.GetString("UserIds", "");
        return new List<string>(storedIds.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));
    }
    public static List<string> GetAllUserIds()
    {
        return GetAllStoredUserIds();
    }

    // ========== 프로필 이미지 반환 함수 ==========
    public static Sprite GetUserProfileSprite(int profileNum)
    {
        if (ProfileSprites == null || ProfileSprites.Length == 0)
        {
            Debug.LogWarning("[SessionManager] 프로필 스프라이트가 설정되지 않았습니다.");
            return null;
        }

        if (profileNum >= 0 && profileNum < ProfileSprites.Length)
        {
            return ProfileSprites[profileNum];
        }
        else
        {
            Debug.LogWarning($"[SessionManager] 잘못된 프로필 번호 요청: {profileNum}");
            return null;
        }
    }

    // ========== 새로운 유저 ID 저장 ==========
    private static void SaveUserId(string userId)
    {
        List<string> userIds = GetAllStoredUserIds();
        if (!userIds.Contains(userId))
        {
            userIds.Add(userId);
            PlayerPrefs.SetString("UserIds", string.Join(",", userIds));
            PlayerPrefs.Save();
        }
    }

    // ========== 유저 ID 삭제 ==========
    private static void RemoveUserId(string userId)
    {
        List<string> userIds = GetAllStoredUserIds();
        if (userIds.Contains(userId))
        {
            userIds.Remove(userId);
            PlayerPrefs.SetString("UserIds", string.Join(",", userIds));
            PlayerPrefs.Save();
        }
    }

    // ========== 유저 세션 업데이트 (예: 코인/급수 변경 후) ==========
    public static void UpdateSession(string userId, int coins, int grade, int rankPoint)
    {
        if (userSessions.ContainsKey(userId))
        {
            userSessions[userId].Coins = coins;
            userSessions[userId].Grade = grade;
            userSessions[userId].RankPoint = rankPoint;
            SaveUserSession(userId); // 즉시 저장
            Debug.Log($"세션 업데이트: {userId} (코인: {coins}, 급수: {grade}, 포인트: {rankPoint})");
        }
    }

    public static void AddWin(string userId)
    {
        var user = GetSession(userId);
        if (user != null)
        {
            user.WinCount++;
            SaveUserSession(userId);
            Debug.Log($"승리 기록: {user.Nickname} - 총 승리 {user.WinCount}회");
        }
    }

    public static void AddLose(string userId)
    {
        var user = GetSession(userId);
        if (user != null)
        {
            user.LoseCount++;
            SaveUserSession(userId);
            Debug.Log($"패배 기록: {user.Nickname} - 총 패배 {user.LoseCount}회");
        }
    }
}