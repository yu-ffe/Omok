using UnityEngine;
using workspace.YU__FFE.Scripts.Server;
using workspace.YU__FFE.Scripts.Server.Network;

namespace workspace.YU__FFE.Scripts.User {
    public class NetworkHandler {

        // ========== 세션 추가 및 저장 ==========
        public void SetUserData(string userId, string nickname, int profileNum,
                                int coins, int grade, int rankPoint, int winCount, int loseCount) {
            PlayerManager.Instance.playerData = new PlayerData(nickname, profileNum, coins, grade, rankPoint, winCount, loseCount);
            Debug.Log($"세션 추가: {userId} - {nickname}");

            SaveUserData(); // 저장
        }

        // ========== 특정 유저 데이터 저장 (Json 방식) ==========
        // SaveUserSession(string userId) -> SaveUserSession()
        private void SaveUserData() {
            // 세션 -> SessionManager, 
            NetworkManager.Instance.SaveUserDataRequest(
                (success, message) => {
                    if (success) {
                        Debug.Log("성공: " + message);
                    }
                    else {
                        Debug.LogError("실패: " + message);
                    }
                });
        }


        // ========== 유저 세션 업데이트 (예: 코인/급수 변경 후) ==========
        public static void UpdateData(string userId, int coins, int grade, int rankPoint) {
            // if (userSessions.ContainsKey(userId)) {
            //     userSessions[userId].Coins = coins;
            //     userSessions[userId].Grade = grade;
            //     userSessions[userId].RankPoint = rankPoint;
            //     SaveUserData(userId); // 즉시 저장
            //     Debug.Log($"세션 업데이트: {userId} (코인: {coins}, 급수: {grade}, 포인트: {rankPoint})");
            // }
        }

        public static void AddWin(string userId) {
            // var user = GetSession(userId);
            // if (user != null) {
            //     user.WinCount++;
            //     SaveUserData(userId);
            //     Debug.Log($"승리 기록: {user.Nickname} - 총 승리 {user.WinCount}회");
            // }
        }

        public static void AddLose(string userId) {
            // var user = GetSession(userId);
            // if (user != null) {
            //     user.LoseCount++;
            //     SaveUserData(userId);
            //     Debug.Log($"패배 기록: {user.Nickname} - 총 패배 {user.LoseCount}회");
            // }
        }

    }
}
