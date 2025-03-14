using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KimHyeun
{
    public class RankingManager : Singleton<RankingManager>
    {
        [Header("랭킹 스크롤 뷰 필수 할당")]
        [SerializeField] ScrollViewSet scrollViewSet;

        List<Sprite> profileSpriteList = new List<Sprite>();
        List<string> nickNameList = new List<string>();
        List<int> GradeList = new List<int>();
        List<int> winList = new List<int>();
        List<int> loseList = new List<int>();



        public void SetScrollView(ScrollViewSet scrollViewSet)
        {
            this.scrollViewSet = scrollViewSet;
        }


        public void GetUserData() // 랭킹 팝업 오픈 시 호출
        {
            ResetData(); // 초기화


            // 모든 유저 id 찾기, 해당 유저들의 정보(딕셔너리 형식) 접근
            // 유저 데이터 불러오기 - 회원가입, 프로필 연동 (닉네임, 프로필 이미지, 급수, 승, 패)
            List<string> userIdList = SessionManager.GetAllUserIds();

            SortingAndSave(userIdList); // 모든 아이디를 전달

            scrollViewSet.StageSelectPopSet(GetMaxCellNum());
        }

        void SortingAndSave(List<string> userIdList) // 급수 기반 정렬 하여 보여줄 데이터 목록 구성
        {
            // 사용자 정보를 저장할 리스트 (Grade를 기준으로 정렬할 것)
            List<(Sprite Profile, string Nickname, int Grade, int Win, int Lose)> userDataList = new();

            for (int i = 0; i < userIdList.Count; i++)
            {
                UserSession userSession = SessionManager.GetSession(userIdList[i]);

                userDataList.Add((
                    SessionManager.GetUserProfileSprite(userSession.ProfileNum),
                    userSession.Nickname,
                    userSession.Grade,
                    userSession.WinCount,
                    userSession.LoseCount
                ));
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
            foreach (var userData in userDataList)
            {
                profileSpriteList.Add(userData.Profile);
                nickNameList.Add(userData.Nickname);
                GradeList.Add(userData.Grade);
                winList.Add(userData.Win);
                loseList.Add(userData.Lose);
            }
        }







        public float GetWinRate(int winCount, int loseCount) // 승률 반환 (일반 계산용)
        {
            return (winCount + loseCount == 0) ? 0 : (winCount / (float)(winCount + loseCount)) * 100;
        }






        void ResetData()
        {
            profileSpriteList.Clear();
            nickNameList.Clear();
            GradeList.Clear();
            winList.Clear();
            loseList.Clear();
        }








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


        public float GetWinRate(int index) // 승류 반환 (여러 유저 계산용)
        {
            int wins = GetWin(index);
            int losses = GetLose(index);

            int totalGames = wins + losses;
            if (totalGames == 0) return 0f; // 경기 기록이 없을 경우 0% 반환

            float winRate = (wins / (float)totalGames) * 100f;
            return Mathf.Round(winRate * 100) / 100; // 소수점 2자리 반올림
        }







        /*
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

        */
    }
}


