using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        // PlayerData userSession = SessionManager.GetSession(SessionManager.id);
        // TODO: 서버에서 플레이어 데이터 가져오기
        PlayerData playerData = null;

        if (playerData != null)
        {
            coinText.text = playerData.coins.ToString() + " 코인";
            // TODO: 이거는 서버필요없음. UI관련 메니저라던가 하나 있으면 좋을듯
            // profile_Image.sprite = SessionManager.GetUserProfileSprite(playerData.profileNum);
            profile_Image.sprite = null;
            winText.text = playerData.winCount.ToString() + " 승";
            loseText.text = playerData.loseCount.ToString() + " 패";

            // 승률 계산
            float winRate = RankingManager.Instance.GetWinRate(playerData.winCount, playerData.loseCount);
            winRateText.text = winRate.ToString("F2") + "%";  // 소수점 2자리까지 표시

            nickNameText.text = playerData.nickname;
            gradeText.text = playerData.grade.ToString() + " 급";
        }

        else
        {
            Debug.LogError("현재 로그인된 유저 정보가 없습니다.");
        }
    }



    void ButtonInfoSet()
    {
        ButtonClickListenerSet(); // 버튼 클릭 이벤트 할당
        ButtonTextSet(); // 버튼 텍스트 할당
    }


    void ButtonClickListenerSet()
    {
        // 기존에 있는 Button 컴포넌트를 가져오고, 없으면 추가
        Button gameStartBtn = gameStartButton.GetComponent<Button>() ?? gameStartButton.AddComponent<Button>();
        Button recordBtn = recordButton.GetComponent<Button>() ?? recordButton.AddComponent<Button>();
        Button rankingBtn = rankingButton.GetComponent<Button>() ?? rankingButton.AddComponent<Button>();
        Button shopBtn = shopButton.GetComponent<Button>() ?? shopButton.AddComponent<Button>();
        Button settingBtn = settingButton.GetComponent<Button>() ?? settingButton.AddComponent<Button>();

        // 기존 리스너 제거 (중복 방지)
        gameStartBtn.onClick.RemoveAllListeners();
        recordBtn.onClick.RemoveAllListeners();
        rankingBtn.onClick.RemoveAllListeners();
        shopBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.RemoveAllListeners();

        // 새로운 클릭 리스너 추가
        gameStartBtn.onClick.AddListener(() => { MainButtonClickManager.Instance.OnClick_GameStartButton(); });
        recordBtn.onClick.AddListener(() => { MainButtonClickManager.Instance.OnClick_RecordButton(); });
        rankingBtn.onClick.AddListener(() => { MainButtonClickManager.Instance.OnClick_RankingButton(); });
        shopBtn.onClick.AddListener(() => { MainButtonClickManager.Instance.OnClick_ShopButton(); });
        settingBtn.onClick.AddListener(() => { MainButtonClickManager.Instance.OnClick_SettingButton(); });
    }

    void ButtonTextSet()
    {
        gameStartButtonText.text = "대국 시작";
        recordButtonText.text = "내 기보";
        rankingButtonText.text = "랭킹";
        shopButtonText.text = "상점";
        settingButtonText.text = "설정";
    }
}
