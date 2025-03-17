using UnityEngine.Serialization;

namespace workspace.YU__FFE.Scripts.User {
    public class PlayerManager : Singleton<PlayerManager> {
        [FormerlySerializedAs("userData")]
        public PlayerData playerData;

        public void UpdateUserData(int coins, int grade, int rankPoint, int winCount, int loseCount) {
            playerData.coins = coins;
            playerData.grade = grade;
            playerData.rankPoint = rankPoint;
            playerData.winCount = winCount;
            playerData.loseCount = loseCount;
        }
    }
}
