using Commons.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : Singleton<PlayerManager> {
    public PlayerData playerData = new PlayerData();
    
    public bool IsLoggedIn => playerData != null && !string.IsNullOrEmpty(playerData.nickname);

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
    
    public void AddCoins(int amount)
    {
        playerData.coins += amount;
        Debug.Log($"[PlayerManager] 현재 코인: {playerData.coins}");

        // (선택 사항) PlayerPrefs에도 저장하여 로컬에서도 반영
        PlayerPrefs.SetInt("PlayerCoins", playerData.coins);
        PlayerPrefs.Save();
    }
    
    public void RefreshUserData()
    {
        Debug.Log("[PlayerManager] 유저 데이터 새로고침 시작");
        Instance.StartCoroutine(UpdateUserData());
    }
}
