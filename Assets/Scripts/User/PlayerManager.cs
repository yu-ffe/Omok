using UnityEngine.Serialization;

public class PlayerManager : Singleton<PlayerManager> {
    public PlayerData playerData = new PlayerData();

    public void UpdateUserData(int coins = 1000, int grade = 8, int rankPoint = 0, int winCount = 0, int loseCount = 0) {
        
        NetworkManager.GetUserInfoRequest(response => {
            playerData.coins = coins;
            playerData.grade = grade;
            playerData.rankPoint = rankPoint;
            playerData.winCount = winCount;
            playerData.loseCount = loseCount;
        });
    }
}
