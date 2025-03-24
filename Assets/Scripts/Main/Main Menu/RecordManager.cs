using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordManager : UI_Panel
{
    public static RecordManager Instance { get; private set; }
    
    [Header("기보 스크롤 뷰 필수 할당")]
    [SerializeField] ScrollViewSet scrollViewSet;

    List<Sprite> resultSpriteList = new List<Sprite>();
    List<string> recordNameList = new List<string>();
    List<string> nickNameList = new List<string>();
    List<int> dateList = new List<int>();
    
    public Button btnClose;

    public void SetScrollView(ScrollViewSet scrollViewSet)
    {
        this.scrollViewSet = scrollViewSet;
    }
    
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        UI_Manager.Instance.AddPanel(UI_Manager.PanelType.Record, this);
        btnClose.onClick.AddListener(Hide);
        gameObject.SetActive(false);
        
        // 테스트
        GetRecordData();
    }

    public void GetRecordData() // 기보 팝업 오픈 시 호출
    {
        ResetData(); // 초기화



        // 기보 데이터 불러오기 필요  (playerpref 기록 불러오기)

        /*
        // 특정 기보 불러오기 (예: 3번째 기보)
        RecordData loadedRecord = GameRecorder.LoadGameRecord(3);
        if (loadedRecord != null)
        {
            Debug.Log($"불러온 기록: {loadedRecord.Nickname} / {loadedRecord.Date} / {loadedRecord.Result}");
        }

        // 모든 저장된 기보 목록 가져오기
        List<RecordData> allRecords = GameRecorder.GetAllGameRecords();
        foreach (var record in allRecords)
        {
            Debug.Log($"기보: {record.Nickname} / {record.Date} / {record.Result}");
        }

        // 특정 기보 삭제 (예: 2번째 기보 삭제)
        GameRecorder.DeleteGameRecord(2);
        */


        // scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }


    // TODO (기보 구현 후) 기보 플레이 호출 기능 (기보 시스템 구축 대기)

    // TODO (기보 구현 후) 기보 제거 기능
    public void RemoveRecord(int index)
    {
        Debug.Log($"{index}인덱스 기보 제거(RecordManager)");
    }


















    void ResetData()
    {
        resultSpriteList.Clear();
        recordNameList.Clear();
        nickNameList.Clear();
        dateList.Clear();
    }

    public int GetMaxCellNum()
    {
        return resultSpriteList.Count;
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

    // TODO (기보 구현 후)날짜 형식 변환 필요
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













































    public override void Show()
    {
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);
        gameObject.SetActive(true);
        GetRecordData();
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        UI_Manager.Instance.Panels[UI_Manager.PanelType.Main].gameObject.SetActive(true);
    }

    public override void OnEnable()
    {
    }

    public override void OnDisable()
    {
    }
}

