using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    [SerializeField] ScrollViewSet scrollViewSet;

    List<Sprite> resultSpriteList;
    List<string> recordNameList;
    List<string> nickNameList;
    List<int> dateList;

    private void Start()
    {
        resultSpriteList = new List<Sprite>();
        recordNameList = new List<string>();
        nickNameList = new List<string>();
        dateList = new List<int>();
    }

    public void GetRecordData() // 기보 팝업 오픈 시 호출
    {
        ResetData(); // 초기화



        // 기보 데이터 불러오기 필요  (playerpref 기록 불러오기)




        scrollViewSet.StageSelectPopSet();
    }


    // TODO 기보 플레이 호출 기능





    void ResetData()
    {
        resultSpriteList.Clear();
        recordNameList.Clear();
        nickNameList.Clear();
        dateList.Clear();
    }


    public Sprite GetSprite(int index)
    {
        if (resultSpriteList.Count > index)
        {
            return resultSpriteList[index];
        }

        else
        {
            return null;
        }
    }

    public string GetRecordName(int index)
    {
        if (recordNameList.Count > index)
        {
            return recordNameList[index];
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

    public int GetDate(int index)
    {
        if (dateList.Count > index)
        {
            return dateList[index];
        }

        else
        {
            return 0;
        }
    }










    private static RecordManager _instance;

    public static RecordManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RecordManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(RecordManager).Name;
                    _instance = obj.AddComponent<RecordManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as RecordManager;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
