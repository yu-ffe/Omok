using Commons.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : UI_Panel
{
    public static RankingManager Instance { get; private set; }
    
    [Header("랭킹 스크롤 뷰 필수 할당")]
    [SerializeField] ScrollViewSet scrollViewSet;

    // List<Sprite> profileSpriteList = new List<Sprite>();
    List<Ranking> playerLankingList;

    public Button btnClose;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Ranking, this);
        btnClose.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void SetScrollView(ScrollViewSet scrollViewSet)
    {
        this.scrollViewSet = scrollViewSet;
    }


    public void GetUserData() // 랭킹 팝업 오픈 시 호출
    {
        // ResetData(); // 초기화


        StartCoroutine(NetworkManager.GetRankingRequest((rankings) =>
        {
            playerLankingList = rankings;
            scrollViewSet.StageSelectPopSet(GetMaxCellNum());
            
        }));
        // 모든 유저 id 찾기, 해당 유저들의 정보(딕셔너리 형식) 접근
        // 유저 데이터 불러오기 - 회원가입, 프로필 연동 (닉네임, 프로필 이미지, 급수, 승, 패)
        // TODO: 서버에서 모든 유저 랭킹 가져오기 -> 이건 아직 안만들꺼임
        // TODO: 서버에서 랭킹 업데이트 -> 랭킹만 가져오기로 동작 예정
        // List<string> userIdList = SessionManager.GetAllUserIds();
        // List<string> userIdList = null;
        
        // SortingAndSave(userIdList); // 모든 아이디를 전달

    }

    void SortingAndSave(List<string> userIdList) // 급수 기반 정렬 하여 보여줄 데이터 목록 구성
    {
        // 사용자 정보를 저장할 리스트 (Grade를 기준으로 정렬할 것)
        List<(Sprite Profile, string Nickname, int Grade, int Win, int Lose)> userDataList = new();

        for (int i = 0; i < userIdList.Count; i++)
        {
            // TODO: 해당 유저별로 데이터를 서버에서 가져오기
            // PlayerData userSession = SessionManager.GetSession(userIdList[i]);
            //
            // userDataList.Add((
            //     SessionManager.GetUserProfileSprite(userSession.ProfileNum),
            //     userSession.Nickname,
            //     userSession.Grade,
            //     userSession.WinCount,
            //     userSession.LoseCount
            // ));
        }


        // Grade가 같다면 승률이 높은 사람이 앞쪽으로 정렬
        userDataList.Sort((a, b) =>
        {
            if (a.Grade != b.Grade)
                return a.Grade.CompareTo(b.Grade); // Grade 기준 오름차순 정렬

            // 승률 계산 (승리 횟수 / 총 경기 수)
            float winRateA = GetWinRate(a.Win, a.Lose);
            float winRateB = GetWinRate(b.Win, b.Lose);

            return winRateB.CompareTo(winRateA); // 승률 기준 내림차순 정렬
        });

        // 정렬된 데이터를 리스트에 추가
        // foreach (var userData in userDataList)
        // {
        //     profileSpriteList.Add(userData.Profile);
        //     nickNameList.Add(userData.Nickname);
        //     gradeList.Add(userData.Grade);
        //     winList.Add(userData.Win);
        //     loseList.Add(userData.Lose);
        // }
    }

    public float GetWinRate(int winCount, int loseCount) // 승률 반환 (일반 계산용)
    {
        return (winCount + loseCount == 0) ? 0 : (winCount / (float)(winCount + loseCount)) * 100;
    }


    void ResetData()
    {
        // profileSpriteList.Clear();
        // nickNameList.Clear();
        // gradeList.Clear();
        // winList.Clear();
        // loseList.Clear();
    }

    public int GetMaxCellNum() => playerLankingList.Count;
    public Ranking GetRanking(int index) => (playerLankingList.Count > index) ? playerLankingList[index] : null;

    // public Sprite GetSprite(int index) => (playerLankingList..Count > index) ? profileSpriteList[index] : null;
    // public string GetName(int index) => (nickNameList.Count > index) ? nickNameList[index] : null;
    // public int GetGrade(int index) => (gradeList.Count > index) ? gradeList[index] : 0;
    // public int GetWin(int index) => (winList.Count > index) ? winList[index] : 0;
    // public int GetLose(int index) => (loseList.Count > index) ? loseList[index] : 0;


    public float GetWinRate(int index) // 승류 반환 (여러 유저 계산용)
    {
        Ranking ranking = playerLankingList[index];
        // int wins = GetWin(index);
        // int losses = GetLose(index);
        int wins = ranking.WinCount;
        int losses = ranking.LoseCount;

        int totalGames = wins + losses;
        if (totalGames == 0) return 0f; // 경기 기록이 없을 경우 0% 반환

        float winRate = (wins / (float)totalGames) * 100f;
        return Mathf.Round(winRate * 100) / 100; // 소수점 2자리 반올림
    }

    public void LoadRankingData()
    {
        ResetData();
        // TODO: 서버 연동 필요
        // List<string> userIdList = SessionManager.GetAllUserIds();
        StartCoroutine(NetworkManager.GetRankingRequest((rankings) =>
        {
            playerLankingList = rankings;
            scrollViewSet.StageSelectPopSet(rankings.Count);
            
        }));
    }
    
    public override void Show()
    {
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);
        gameObject.SetActive(true);
        LoadRankingData();
    }

    public override void Hide()
    {
        SoundManager.Instance.ButtonClickSound();//버튼 클릭음
        gameObject.SetActive(false);
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);
    }

    public override void OnEnable() {
    }

    public override  void OnDisable() {
    }
}

