using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WB;

namespace WB
{
    public class MainPanel : UI_Panel
    {
        public Image imgUserPortrait;
        public TextMeshProUGUI txtCoin;
        public TextMeshProUGUI txtUserName;

        SessionManager.UserSession UseData => SessionManager.GetSession(SessionManager.currentUserId);

        void Start()
        {
            UI_Manager.Instance.AddCallback("userinfo", ResfreshUserInfo);
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
            //Coin
            txtCoin.text = UseData.Coins.ToString();
            //유저정보 새로고침
            txtUserName.text = $"{UseData.Grade}급 {UseData.Nickname}";
            //TODO sprite 어디에서?
            // imgUserPortrait.sprite = userData.ProfileNum
        }

    }

}