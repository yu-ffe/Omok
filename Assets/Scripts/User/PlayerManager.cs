using System.Collections;
using UnityEngine.Serialization;

public class PlayerManager : Singleton<PlayerManager> {
    public PlayerData playerData = new PlayerData();

    public IEnumerator UpdateUserData() {

        StartCoroutine(NetworkManager.GetUserInfoRequest(response => {
            playerData.coins = response.Coins;
            playerData.grade = response.Grade;
            playerData.rankPoint = response.RankPoint;
            playerData.winCount = response.WinCount;
            playerData.loseCount = response.LoseCount;
        }));
        yield break;
    }
    
}
