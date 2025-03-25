using System;
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
    List<string> dateList = new List<string>();

    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite loseSprite;
    [SerializeField] Sprite drawSprite;

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

        // 모든 저장된 기보 목록 가져오기
        List<RecordData> allRecords = GameRecorder.GetAllGameRecords();


        SortingAndSet(allRecords);




        scrollViewSet.StageSelectPopSet(GetMaxCellNum());
    }


    // 기보 플레이 호출 기능
    public void RecordReplay(int index)
    {
        Debug.Log($"{index}인덱스 기보 재생(RecordManager)");

        // 특정 기보 불러오기 (예: 3번째 기보)
        RecordData loadedRecord = GameRecorder.LoadGameRecord(index);
        if (loadedRecord != null)
        {
            Debug.Log($"불러온 기록: {loadedRecord.Nickname} / {loadedRecord.Date} / {loadedRecord.Result}");

        }

        ReplayShow(loadedRecord);
    }



    // 기보 제거 기능
    public void RemoveRecord(int index)
    {
        Debug.Log($"{index}인덱스 기보 제거(RecordManager)");


        GameRecorder.DeleteGameRecord(index);
        GetRecordData(); // 리로드
    }


    void SortingAndSet(List<RecordData> recordDatas) // 날짜 기반 정렬
    {
        // 사용자 정보를 저장할 리스트 (Grade를 기준으로 정렬할 것)
        List<(Sprite resultSprite, string recordName, string nickName, DateTime date)> recordDataList = new();

        for (int i = 0; i < recordDatas.Count; i++)
        {
            if (!Enum.TryParse(recordDatas[i].Result, out GameResult resultEnum))
            {
                Debug.LogWarning($"GameResult 변환 실패: {recordDatas[i].Result}");
                continue; // 변환 실패 시 무시
            }

            if (!DateTime.TryParse(recordDatas[i].Date, out DateTime parsedDate))
            {
                Debug.LogWarning($"날짜 변환 실패: {recordDatas[i].Date}");
                continue;
            }


            recordDataList.Add((
                GetResultSprite(resultEnum),
                $"{i}번 기보",
                recordDatas[i].Nickname,
                parsedDate
            ));
        }


        // 최신순 날짜로 정렬 (내림차순)
        recordDataList.Sort((a, b) => b.date.CompareTo(a.date));


        // 정렬된 데이터를 리스트에 추가
        foreach (var record in recordDataList)
        {
            resultSpriteList.Add(record.resultSprite);
            recordNameList.Add(record.recordName);
            nickNameList.Add(record.nickName);
            dateList.Add(record.date.ToString("yyyy-MM-dd HH:mm:ss")); // 날짜 포맷
        }
    }


    Sprite GetResultSprite(GameResult gameResult)
    {
        if(gameResult == GameResult.Win)
        {
            return winSprite;
        }

        else if (gameResult == GameResult.Lose)
        {
            return loseSprite;
        }

        else if (gameResult == GameResult.Draw)
        {
            return drawSprite;
        }

        return null;
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

    // 날짜 형식 변환
    public string GetDate(int index)
    {
        if (DateTime.TryParse(dateList[index], out DateTime parsedDate))
        {
            return parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            Debug.LogWarning($"날짜 변환 실패: {dateList[index]}");
            return "Invalid Date";
        }
    }















    public void ReplayShow(RecordData recordData)
    {


        // TODO 기보 재생
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

