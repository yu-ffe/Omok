using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : WB.UI_Panel
{
    [Header("좌측 프로필")]
    public Image imgProfileLeft;
    public TextMeshProUGUI txtNickNameLeft;

    [Header("우측 프로필")]
    public Image imgProfileRight;
    public TextMeshProUGUI txtNickNameRight;


    [Header("시간/턴 정보")]
    public TextMeshPro txtTimer;
    public Image[] imgGameTurn = new Image[2];

    bool isComponentsConnected = false;


    public override void Show()
    {
        if (!isComponentsConnected)
            FindComponents();

        LoadProfile();
    }
    public override void Hide()
    {

    }

    public override void OnDisable()
    {

    }

    public override void OnEnable()
    {

    }

    /// <summary> 컴포넌트와 버튼 이벤트 연결 스크립트 </summary>
    void FindComponents()
    {
        //0 Profile, 1 Info, 2 Button
        var parentsProfile = transform.GetChild(0);
        var parentsInfo = transform.GetChild(1);
        var parentsButton = transform.GetChild(2);
        imgProfileLeft = parentsProfile.GetChild(0).GetComponent<Image>();
        txtNickNameLeft = imgProfileLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        imgProfileRight = parentsProfile.GetChild(1).GetComponent<Image>();
        txtNickNameRight = imgProfileRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        txtTimer = parentsInfo.GetChild(0).GetComponent<TextMeshPro>();
        imgGameTurn[0] = parentsInfo.GetChild(1).GetComponent<Image>();
        imgGameTurn[1] = parentsInfo.GetChild(2).GetComponent<Image>();
        isComponentsConnected = true;

        var giveup = parentsButton.GetChild(0).GetComponent<Button>();
        var endturn = parentsButton.GetChild(0).GetComponent<Button>();

        giveup.onClick.AddListener(Button_GiveUp);
        endturn.onClick.AddListener(Button_EndOfTurn);
    }


    /// <summary> 양쪽 게임 유저의 프로필 사진와 닉네임 가져옵니다 </summary>
    void LoadProfile()
    {

    }


    void LoadGameState()
    {

    }








    /// <summary> 기권버튼 이벤트 </summary>
    public void Button_GiveUp()
    {

    }

    /// <summary> 착수버튼 이벤트 </summary>
    public void Button_EndOfTurn()
    {

    }

}
