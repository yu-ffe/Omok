using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using KimHyeun;
using MJ;
using workspace.YU__FFE.Scripts;

namespace WB
{
    public class GamePanel : WB.UI_Panel
    {
        [Header("좌측 프로필")]
        public Image imgProfileLeft;
        public TextMeshProUGUI txtNickNameLeft;

        [Header("우측 프로필")]
        public Image imgProfileRight;
        public TextMeshProUGUI txtNickNameRight;


        [Header("시간/턴 정보")]
        public TextMeshProUGUI txtTimer;
        public Image imgLeftTime;
        public Image[] imgGameTurn = new Image[2];

        bool isComponentsConnected = false;


        void Start()
        {
            if (!isComponentsConnected)
                FindComponents();

            UI_Manager.Instance.AddPanel(WB.UI_Manager.PanelType.Game, this);
            UI_Manager.Instance.AddCallback("turn", TurnStateRefresh);
            UI_Manager.Instance.AddCallback("time", TimeRefresh);
        }

        public override void Show()
        {


            LoadProfile();

            LoadGameState();

            gameObject.SetActive(true);
        }
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {
        }

        [ContextMenu("Components Connect")]
        /// <summary> 컴포넌트와 버튼 이벤트 연결 스크립트 </summary>
        void FindComponents()
        {
            //0 Profile, 1 Info, 2 Button
            var parentsProfile = transform.GetChild(0);
            var parentsInfo = transform.GetChild(1);
            var parentsButton = transform.GetChild(2);
            imgProfileLeft = parentsProfile.GetChild(0).GetChild(0).GetComponent<Image>();
            txtNickNameLeft = imgProfileLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            imgProfileRight = parentsProfile.GetChild(1).GetChild(0).GetComponent<Image>();
            txtNickNameRight = imgProfileRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            imgLeftTime = parentsInfo.GetChild(0).GetComponent<Image>();
            txtTimer = imgLeftTime.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            imgGameTurn[0] = parentsInfo.GetChild(1).GetComponent<Image>();
            imgGameTurn[1] = parentsInfo.GetChild(2).GetComponent<Image>();
            isComponentsConnected = true;

            var giveup = parentsButton.GetChild(0).GetComponent<Button>();
            var endturn = parentsButton.GetChild(1).GetComponent<Button>();

            giveup.onClick.RemoveAllListeners();
            endturn.onClick.RemoveAllListeners();
            giveup.onClick.AddListener(Button_GiveUp);
            endturn.onClick.AddListener(Button_EndOfTurn);

            isComponentsConnected = true;
        }


        /// <summary> 양쪽 게임 유저의 프로필 사진와 닉네임 가져옵니다 </summary>
        void LoadProfile()
        {
            // Sprite sprite_Left = SessionManager.GetUserProfileSprite()
            // Sprite sprite_Right = SessionManager.GetUserProfileSprite()

            // imgProfileLeft.sprite = sprite_Left;
            // imgProfileRight.sprite = sprite_Right;
            // txtNickNameLeft.text = nickName_Left;
            // txtNickNameRight.text = nickName_Right;
        }


        void LoadGameState()
        {

        }


        void TurnStateRefresh()
        {

            //게임 로직에 따라 변경
            Debug.Log("Game Panel : TurnStateRefresh");
            //Temp Test value
            int now = 0;
            imgGameTurn[0].color = now == 0 ? Color.white : Color.gray;
            imgGameTurn[1].color = now == 1 ? Color.white : Color.gray;
        }

        void TimeRefresh()
        {
            //게임 로직에 따라 변경
            //Temp Test value
            float leftTime = Random.Range(0f, 30f);
            imgLeftTime.fillAmount = leftTime / 30f;
            txtTimer.text = Mathf.RoundToInt(leftTime).ToString();
        }


        void ExitToMain()
        {

        }


















        /// <summary> 기권버튼 이벤트 </summary>
        public void Button_GiveUp()
        {
            UI_Manager.Instance.popup.Show(
            msg: "기권 하시겠습니까?",
            "기권", "취소",
            okAction: () =>
            {
                SceneManager.Instance.LoadSceneAsync("Main");
            },
            cancelAction: () =>
            {

            }
            );
        }

        /// <summary> 착수버튼 이벤트 </summary>
        public void Button_EndOfTurn()
        {

        }

    }
}

