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
        [SerializeField] TMP_Text gameStartText;
        [SerializeField] GameObject recordButton;
        [SerializeField] TMP_Text recordText;
        [SerializeField] GameObject rankingButton;
        [SerializeField] TMP_Text rankingText;
        [SerializeField] GameObject shopButton;
        [SerializeField] TMP_Text shopText;
        [SerializeField] GameObject settingButton;
        [SerializeField] TMP_Text settingText;


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
                winRateText.text = userSession.LoseCount.ToString();

                // 승률 계산 (승률 = 승리 수 / (승리 수 + 패배 수) * 100)
                float winRate = (userSession.WinCount + userSession.LoseCount) > 0
                    ? (float)userSession.WinCount / (userSession.WinCount + userSession.LoseCount) * 100
                    : 0;

                // 소수점 2자리까지 표시
                winRateText.text = winRate.ToString("F2") + "%";
            }

            else
            {
                Debug.LogError("현재 로그인된 유저 정보가 없습니다.");
            }
        }
    }
}
