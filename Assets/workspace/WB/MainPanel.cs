using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using WB;

public class MainPanel : UI_Panel
{
    public Image imgUserPortrait;

    void Start()
    {
        UI_Manager.Instance.AddPanel(panelType, this);
    }
    public override void Show()
    {
        gameObject.SetActive(true);
        ResfreshUserInfo();
    }
    public override void Hide()
    {
        gameObject.SetActive(false);

    }


    public override void OnEnable()
    {
        UI_Manager.Instance.AddCallback("UserInfo", ResfreshUserInfo);
    }
    public override void OnDisable()
    {
        UI_Manager.Instance.RemoveCallback("UserInfo");
    }

    void ResfreshUserInfo()
    {
        //유저정보 새로고침

    }


}
