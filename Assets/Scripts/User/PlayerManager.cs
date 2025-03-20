using Commons.Models;
using System.Collections;
using UnityEngine.Serialization;

public class PlayerManager : Singleton<PlayerManager> {
    public PlayerData playerData = new PlayerData();

    // 지금은 비동기지만 필요 없을 가능성이 높음.
    // 임시 변경
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

    public void SetPlayerData(PlayerDataResponse data) {
        playerData.nickname = data.Nickname;
        playerData.profileNum = data.ProfileNum;
        playerData.coins = data.Coins;
        playerData.grade = data.Grade;
        playerData.rankPoint = data.RankPoint;
        playerData.winCount = data.WinCount;
        playerData.loseCount = data.LoseCount;
    }
    
}
