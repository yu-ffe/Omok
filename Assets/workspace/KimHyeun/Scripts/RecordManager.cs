using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    [SerializeField] ScrollViewSet scrollViewSet;

    public Sprite resultSprite;
    public string recordName;
    public string nickName;
    public int date;



    public void GetRecordData() // 기보 팝업 오픈 시 호출
    {
        // 기보 데이터 불러오기 필요  (playerpref 기록 불러오기)




        scrollViewSet.StageSelectPopSet();
    }


    // TODO 기보 플레이 호출 기능



    public Sprite GetSprite()
    {
        return resultSprite;
    }

    public string GetRecordName()
    {
        return recordName;
    }

    public string GetName()
    {
        return nickName;
    }

    public int GetDate()
    {
        return date;
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
