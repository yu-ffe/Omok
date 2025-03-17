using UnityEngine.Serialization;

namespace workspace.YU__FFE.Scripts.User {
    public class PlayerManager : Singleton<PlayerManager> {
        public PlayerData playerData;

        public void UpdateUserData(int coins = 1000, int grade = 8, int rankPoint = 0, int winCount = 0, int loseCount = 0) {
            playerData.coins = coins;
            playerData.grade = grade;
            playerData.rankPoint = rankPoint;
            playerData.winCount = winCount;
            playerData.loseCount = loseCount;
        }
    }
}
