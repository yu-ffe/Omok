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
        panelKey = "Main";
        UI_Manager.Instance.AddPanel(panelKey, this);
    }
    public override void Show()
    {
        gameObject.SetActive(true);
        ResfreshUserInfo();
    }
    public override void Hide()
    {
    }


    public override void OnEnable()
    {
        UI_Manager.Instance.AddCallback(panelKey, ResfreshUserInfo);
    }
    public override void OnDisable()
    {
        UI_Manager.Instance.RemoveCallback(panelKey);
    }

    void ResfreshUserInfo()
    {
        //유저정보 새로고침

    }


}
