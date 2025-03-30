using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserScorePopup : MonoBehaviour
{
    public Image[] imgScore;
    public TextMeshProUGUI txtMsgGradeUp;
    public TextMeshProUGUI[] txtRequireScore; //0 prev / 1 next

    const string MSG = "게임만 승리하면 승급합니다.";
    public void ShowScore(int score)
    {
        txtRequireScore[0].text = (-999).ToString();
        txtRequireScore[1].text = 999.ToString();
        int reqUp = 999;

        txtMsgGradeUp.text = reqUp + MSG;
        gameObject.SetActive(true);
    }
}
