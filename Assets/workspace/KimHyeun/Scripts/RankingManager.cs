using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RankingManager : MonoBehaviour
{
    [Header("랭킹 스크롤 뷰 필수 할당")]
    [SerializeField] ScrollViewSet scrollViewSet;

    List<Sprite> profileSpriteList;
    List<string> nickNameList;
    List<int> GradeList;
    List<int> winList;
    List<int> loseList;

    [Header("테스트")]
    public Sprite[] profileSpritesFromInspector; // 인스펙터에서 연결하는 배열

    private void Start()
    {
        profileSpriteList = new List<Sprite>();
        nickNameList = new List<string>();
        GradeList = new List<int>();
        winList = new List<int>();
        loseList = new List<int>();


        // 테스트
        // SessionManager.AddSession("TestId1", "TestNickName1", 0, 1000, 18, 0, 0, 0);
        // SessionManager.AddSession("TestId2", "TestNickName2", 0, 500, 7, 0, 0, 0);
        // SessionManager.AddSession("TestId3", "TestNickName3", 0, 300, 10, 0, 0, 0);
        // SessionManager.AddSession("TestId4", "TestNickName4", 0, 100, 1, 0, 0, 0);

        SessionManager.ProfileSprites = profileSpritesFromInspector;
        SessionManager.LoadAllSessions();
        GetUserData();
    }

    public void GetUserData() // 랭킹 팝업 오픈 시 호출
    {
        ResetData(); // 초기화


        // 모든 유저 id 찾기, 해당 유저들의 정보(딕셔너리 형식) 접근
        // 유저 데이터 불러오기 - 회원가입, 프로필 연동 (닉네임, 프로필 이미지, 급수, 승, 패)
        List<string> userIdList = SessionManager.GetAllUserIds();

        for (int i = 0; i < userIdList.Count; i++)
        {
            SessionManager.UserSession userSession = SessionManager.GetSession(userIdList[i]);
           
            profileSpriteList.Add(SessionManager.GetUserProfileSprite(userSession.ProfileNum));
            nickNameList.Add(userSession.Nickname);
            GradeList.Add(userSession.Grade);
            winList.Add(userSession.WinCount);
            loseList.Add(userSession.LoseCount);
        }

        scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }


    void ResetData()
    {
        profileSpriteList.Clear();
        nickNameList.Clear();
        GradeList.Clear();
        winList.Clear();
        loseList.Clear();
    }
    


    // TODO 급수(level) 기반 랭킹(낮을 수록 상위), 동일 급수 시 승률 우선



    public int GetMaxCellNum()
    {
        return profileSpriteList.Count;
    }

    public Sprite GetSprite(int index)
    {
        if (profileSpriteList.Count > index)
        {
            return profileSpriteList[index];
        }

        else
        {
            return null;
        }
    }

    public string GetName(int index)
    {
        if (nickNameList.Count > index)
        {
            return nickNameList[index];
        }

        else
        {
            return null;
        }
    }

    public int GetGrade(int index)
    {
        if (GradeList.Count > index)
        {
            return GradeList[index];
        }

        else
        {
            return 0;
        }
    }

    public int GetWin(int index)
    {
        if (winList.Count > index)
        {
            return winList[index];
        }

        else
        {
            return 0;
        }
    }

    public int GetLose(int index)
    {
        if (loseList.Count > index)
        {
            return loseList[index];
        }

        else
        {
            return 0;
        }
    }











    private static RankingManager _instance;

    public static RankingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RankingManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(RankingManager).Name;
                    _instance = obj.AddComponent<RankingManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as RankingManager;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
