using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KimHyeun {
    public class ProfileManager : Singleton<ProfileManager>
    {
        [Header("프로필 필수 할당")]
        [SerializeField] TMP_Text coinText;
        [SerializeField] Image profile_Image;
        [SerializeField] TMP_Text winText;
        [SerializeField] TMP_Text loseText;
        [SerializeField] TMP_Text winRateText;
        [SerializeField] TMP_Text nickNameText;
        [SerializeField] TMP_Text gradeText;

        [Header("필수 할당 - 버튼 역할 오브젝트,텍스트")]
        [SerializeField] GameObject gameStartButton;
        [SerializeField] TMP_Text gameStartButtonText;
        [SerializeField] GameObject recordButton;
        [SerializeField] TMP_Text recordButtonText;
        [SerializeField] GameObject rankingButton;
        [SerializeField] TMP_Text rankingButtonText;
        [SerializeField] GameObject shopButton;
        [SerializeField] TMP_Text shopButtonText;
        [SerializeField] GameObject settingButton;
        [SerializeField] TMP_Text settingButtonText;


      


        public void UserInfoShow() 
        {
            UserInfoSet();
        }

        public void ButtonInfoShow() 
        {
            ButtonInfoSet();
        }








        void UserInfoSet()
        {
            UserSession userSession = SessionManager.GetSession(SessionManager.currentUserId);


            if (userSession != null)
            {
                coinText.text = userSession.Coins.ToString() + " 코인";
                profile_Image.sprite = SessionManager.GetUserProfileSprite(userSession.ProfileNum);
                winText.text = userSession.WinCount.ToString() + " 승";
                loseText.text = userSession.LoseCount.ToString() + " 패";

                // 승률 계산
                float winRate = RankingManager.Instance.GetWinRate(userSession.WinCount, userSession.LoseCount);               
                winRateText.text = winRate.ToString("F2") + "%";  // 소수점 2자리까지 표시

                nickNameText.text = userSession.Nickname;
                gradeText.text = userSession.Grade.ToString() + " 급";
            }

            else
            {
                Debug.LogError("현재 로그인된 유저 정보가 없습니다.");
            }
        }



        void ButtonInfoSet() // TODO 버튼 클릭 시 호출 추가
        {
            gameStartButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("게임 시작 버튼 클릭"); });
            recordButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("내 기보 버튼 클릭"); });
            rankingButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("랭킹 버튼 클릭"); });
            shopButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("상점 버튼 클릭"); });
            settingButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("설정 버튼 클릭"); });

            gameStartButtonText.text = "대국 시작";
            recordButtonText.text = "내 기보";
            rankingButtonText.text = "랭킹";
            shopButtonText.text = "상점";
            settingButtonText.text = "설정";
        }
    }
}
