using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataUpdateHandler : MonoBehaviour
{
    private float nextUpdateTime = 0f;
    
    public TMP_Text txtCoin;
    public TMP_Text grade_name;
    public Image image;
    
    // Update is called once per frame
    void Update()
    {

        if (Time.time >= nextUpdateTime) {
            txtCoin.text = PlayerManager.Instance.playerData.coins.ToString() + " 코인";
            grade_name.text = PlayerManager.Instance.playerData.grade.ToString() + "급\n" + PlayerManager.Instance.playerData.nickname;
            image.sprite = PlayerManager.Instance.GetProfileSprites(PlayerManager.Instance.playerData.profileNum);
        // 이렇게 짜면 욕먹을테지만 상관없다.
            // 눈속임만 완벽하면 해결될 일..
            nextUpdateTime = Time.time + 0.1f; // 1초 후 다시 실행
        }
    }
}
