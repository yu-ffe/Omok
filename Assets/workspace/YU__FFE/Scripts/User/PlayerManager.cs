using UnityEngine.Serialization;

namespace workspace.YU__FFE.Scripts.User {
    public class PlayerManager : Singleton<PlayerManager> {
        public UserData userData;

        public void UpdateUserData(int coins, int grade, int rankPoint, int winCount, int loseCount) {
            userData.Coins = coins;
            userData.Grade = grade;
            userData.RankPoint = rankPoint;
            userData.WinCount = winCount;
            userData.LoseCount = loseCount;
        }
    }
}
