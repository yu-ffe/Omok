using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public Sprite profileSprite;
    public string nickName;
    public int level;
    public int win;
    public int lose;



    void GetUserData() // 랭킹 팝업 오픈 시 호출
    {
        // 유저 데이터 불러오기 필요  (playerpref 기록 불러오기)
    }




    // TODO 급수(level) 기반 랭킹(낮을 수록 상위), 동일 급수 시 승률 우선




    public Sprite GetSprite()
    {
        return profileSprite;
    }

    public string GetName()
    {
        return nickName;
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetWin()
    {
        return win;
    }

    public int GetLose()
    {
        return lose;
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
