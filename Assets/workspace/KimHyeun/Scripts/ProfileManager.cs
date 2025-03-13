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

        [Header("버튼 역할 오브젝트,텍스트 필수 할당")]
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


        

        public void UserInfoShow() // TODO 로그인 시 호출, 상점 코인 구매 시 호출, 게임 종료 후 프로필 정보로 돌아가면 호출 -> 프로필 정보 변경 시 호출
        {
            UserInfoSet();
        }

        void UserInfoSet()
        {
            UserSession userSession = SessionManager.GetSession(SessionManager.currentUserId);

            if (userSession != null)
            {
                coinText.text = userSession.Coins.ToString();
                profile_Image.sprite = SessionManager.GetUserProfileSprite(userSession.ProfileNum);
                winText.text = userSession.WinCount.ToString();
                loseText.text = userSession.LoseCount.ToString();

                // 승률 계산
                float winRate = RankingManager.Instance.GetWinRate(userSession.WinCount, userSession.LoseCount);               
                winRateText.text = winRate.ToString("F2") + "%";  // 소수점 2자리까지 표시

                nickNameText.text = userSession.Nickname;
            }

            else
            {
                Debug.LogError("현재 로그인된 유저 정보가 없습니다.");
            }
        }



        public void ButtonInfoShow() // TODO 메인 화면 전환 시 호출 (로그인 시, 게임 종료 후)
        {
            ButtonInfoSet();
        }


        void ButtonInfoSet() // TODO 버튼 클릭 시 호출 추가
        {
            gameStartButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("게임 시작 버튼 클릭"); });
            recordButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("내 기보 버튼 클릭"); });
            rankingButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("랭킹 버튼 클릭"); });
            shopButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("상점 버튼 클릭"); });
            settingButton.AddComponent<Button>().onClick.AddListener(() => { Debug.Log("설정 버튼 클릭"); });

            gameStartButtonText.text = "GameStartButton";
            recordButtonText.text = "RecordButton";
            rankingButtonText.text = "RankingButton";
            shopButtonText.text = "ShopButton";
            settingButtonText.text = "SettingButton";
        }
    }
}
