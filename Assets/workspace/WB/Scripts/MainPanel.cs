using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WB;
using workspace.YU__FFE.Scripts;

namespace WB
{
    public class MainPanel : UI_Panel
    {
        public TextMeshProUGUI txtCoin;
        public Image imgUserPortrait;
        public TextMeshProUGUI txtUserName;

        UserSession UserData => SessionManager.GetSession(SessionManager.currentUserId);
        bool isConnctedCompoenets = false;

        void Start()
        {
            if (!isConnctedCompoenets)
                FindComponents();

            UI_Manager.Instance.AddPanel(panelType, this);
            gameObject.SetActive(false);
        }
        public override void Show()
        {
            ResfreshUserInfo();
            gameObject.SetActive(true);
        }
        public override void Hide()
        {
            gameObject.SetActive(false);

        }


        void FindComponents()
        {
            var root = transform;
            txtCoin = root.GetChild(0).GetComponent<TextMeshProUGUI>();
            imgUserPortrait = root.GetChild(1).GetComponent<Image>();
            txtUserName = root.GetChild(2).GetComponent<TextMeshProUGUI>();

            var buttons = root.GetChild(3).GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() => { OnClick_Menu(index); });
            }

            isConnctedCompoenets = true;
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
            txtCoin.text = UserData.Coins.ToString();
            //유저정보 새로고침
            txtUserName.text = $"{UserData.Grade}급 {UserData.Nickname}";

            imgUserPortrait.sprite = SessionManager.ProfileSprites[UserData.ProfileNum];
        }



        public void OnClick_Menu(int idx)
        {
            switch (idx)
            {
                case 0:
                    Debug.Log("대국 시작");
                    // UI_Manager.Instance.Show(UI_Manager.PanelType.Game);
                    SceneManager.Instance.LoadScene("Game");
                    break;
                case 1:
                    Debug.Log("내 기보");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Record);
                    break;
                case 2:
                    Debug.Log("랭킹");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Ranking);
                    break;
                case 3:
                    Debug.Log("상점");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Shop);
                    break;
                case 4:
                    Debug.Log("설정");
                    UI_Manager.Instance.Show(UI_Manager.PanelType.Option);
                    break;
                default: Debug.Log("???"); break;
            }
        }

    }

}