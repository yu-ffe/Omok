using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSet : MonoBehaviour
{
    // 본 스크립트는 On 되있는 매니저에 부착할 것
    [Header("필수 할당")]
    [SerializeField] GameObject cellPrefab; // content 셀 프리팹
    RectTransform cellRect;
    float cellWidth;  // 각 셀의 너비
    float cellHeight; // 각 셀의 높이

    [SerializeField] GameObject content; // content (부모 오브젝트)
    [SerializeField] RectTransform contentRectTransform;

    [SerializeField] ScrollRect scrollRect;

    List<GameObject> pool = new List<GameObject>(); // 오브젝트 풀링 리스트

    [Header("필수 입력")]
    float spacing;     // 셀 간의 간격
    int totalCells;
    [SerializeField, Tooltip("생성할 셀 수(버퍼 포함)(권장:보이는 셀 + (가로 셀 개수 * 3))")] int createCellCount;
    [SerializeField, Tooltip("가로 셀 개수")] int cellRowCount;


    private float contentWidth; // 콘텐츠 너비
    private float contentHeight; // 콘텐츠 높이
    int lastStartIndex;


    [Header("추가 필요 정보")]
    bool needMore;

   

    public void StageSelectPopSet(int maxCellNum) // 최초 외부 호출
    {
        if (scrollRect.onValueChanged.GetPersistentEventCount() == 0)
        {
            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        spacing = 30f;
        scrollRect.verticalNormalizedPosition = 1f;

        totalCells = maxCellNum;

      

        if (totalCells > 0)
        {
            CellInfoSet();

            UpdateStages(GetStartIndex());
        }
    }



    void CellInfoSet()
    {
        cellRect = cellPrefab.GetComponent<RectTransform>();

        // 셀 정보
        cellWidth = cellRect.sizeDelta.x;
        cellHeight = cellRect.sizeDelta.y;

        // 셀 좌상단 정렬
        cellRect.anchorMin = new Vector2(0, 1);
        cellRect.anchorMax = new Vector2(0, 1);
        cellRect.pivot = new Vector2(0, 1);

        // 코드 지정(인스펙터 할당 가능)
        if (totalCells <= 0)
        {
            Debug.LogError("TotalCells value is 0, Auto Set 1");
            totalCells = 1;
        }

        if (cellRowCount <= 0)
        {
            Debug.LogWarning("CellRowCount value is 0, Auto Set 1");
            cellRowCount = 1;
        }

        if (createCellCount <= 0)
        {
            Debug.LogWarning("CreateCellCount value is 0, Auto Set 1");
            createCellCount = 1;
        }

        ContentSizeSet(); // 스크롤 사이즈 재적용
    }

    void ContentSizeSet()
    {
        contentWidth = (cellWidth + spacing) * cellRowCount - spacing; // 가로 크기 계산 (마지막 열 spacing 제거)

        int totalRows = Mathf.CeilToInt((float)totalCells / cellRowCount);
        contentHeight = (cellHeight + spacing) * totalRows - spacing; // 마지막 줄 아래쪽에 불필요한 spacing 제거

        // content 상단 정렬
        contentRectTransform.anchorMin = new Vector2(0.5f, 1);
        contentRectTransform.anchorMax = new Vector2(0.5f, 1);
        contentRectTransform.pivot = new Vector2(0.5f, 1);

        contentRectTransform.sizeDelta = new Vector2(contentWidth, contentHeight);
        contentRectTransform.anchoredPosition = new Vector2(0, 0);

        createCellCount = Mathf.CeilToInt(contentRectTransform.sizeDelta.y / (cellHeight + spacing)) * cellRowCount;

        if (createCellCount > totalCells) // 생성 셀 개수 보정
        {
            createCellCount = totalCells;
        }


        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < createCellCount; i++) // 위 1줄 아래 2줄 버퍼
        {
            GameObject stageObj = Instantiate(cellPrefab, content.transform);
            stageObj.SetActive(false); // 초기에는 비활성화
            pool.Add(stageObj);
        }

        CellPositionSet(0);
    }


    void CellPositionSet(int startRow)
    {
        // 셀들 위치 설정

        for (int i = 0; i < pool.Count; i++)
        {
            int row = (i + startRow * cellRowCount) / cellRowCount; // 시작 행을 기준으로 행 계산
            int col = (i + startRow * cellRowCount) % cellRowCount; // 열 계산

            float xPos = col * (cellWidth + spacing);   // 가로 위치
            float yPos = -row * (cellHeight + spacing) -20f ; // 세로 위치 (위 → 아래) 첫번째 셀 생성 20아래로 위치

            RectTransform rect = pool[i].GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(xPos, yPos);
            pool[i].SetActive(true); // 초기 화면에 보이도록 활성화
        }
    }





    private void OnScroll(Vector2 scrollPos)
    {
        var startIndex = GetStartIndex();

        if (startIndex > lastStartIndex) // 아래로 생성
        {
            for (int i = 0; i < cellRowCount; i++)
            {
                // 풀 시작 1행을 맨 끝으로 이동
                GameObject stageObj = pool[0];
                pool.RemoveAt(0);
                pool.Add(stageObj);
            }


            CellPositionSet(startIndex / cellRowCount);

            UpdateStages(startIndex); // 셀 위치에 따른 추가 조정
        }

        else if (startIndex < lastStartIndex) // 위로 생성
        {
            // 풀 끝 1행을 맨 위로 이동
            for (int i = 0; i < cellRowCount; i++)
            {
                GameObject stageObj = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                pool.Insert(0, stageObj);
            }


            CellPositionSet(startIndex / cellRowCount);

            UpdateStages(startIndex); // 셀 위치에 따른 추가 조정
        }


        lastStartIndex = startIndex;
    }


    int GetStartIndex() // 보일 셀 시작 인덱스 얻기
    {
        // 보이는 영역의 Rect
        Rect visibleRect = new Rect(
            scrollRect.content.anchoredPosition.x,
            scrollRect.content.anchoredPosition.y,
            contentRectTransform.rect.width,
            contentRectTransform.rect.height
        );



        // 셀 높이와 간격을 반영하여 보이는 영역에서 몇 번째 셀부터 시작하는지 계산
        float totalCellHeight = cellHeight + spacing; // 셀 높이 + 간격
        var startIndex = Mathf.FloorToInt(visibleRect.y / totalCellHeight); // 전체 셀 높이 기준으로 시작 인덱스 계산



        // 셀의 시작 인덱스를 stagesPerRow에 맞게 조정
        startIndex = startIndex * cellRowCount;

        // 맨 위가 아니면 위쪽 버퍼 1줄을 반영 (시작 인덱스 조정)
        startIndex = Mathf.Max(0, startIndex - cellRowCount);

        // 맨 아래가 아니면 아래쪽 버퍼 1줄을 반영
        int totalRows = Mathf.CeilToInt((float)totalCells / cellRowCount);
        int maxStartIndex = Mathf.Max(0, (totalRows - (createCellCount / cellRowCount)) * cellRowCount);
        startIndex = Mathf.Min(startIndex, maxStartIndex);


        return startIndex;
    }


    private void UpdateStages(int startIndex)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            int stageLevel = startIndex + i;

            if (stageLevel >= totalCells)
            {
                pool[i].SetActive(false);
                continue;
            }

            GameObject stageObj = pool[i];

            stageObj.SetActive(true); // 현재 스테이지는 활성화

            CellInfoUpdate(stageObj, i); // 셀정보 업데이트       
        }
    }










  void CellInfoUpdate(GameObject cell, int index)
{
    CellState state = cell.GetComponent<CellState>();
    if (state == null)
    {
        if (state.cellType == CellState.CellType.Ranking)
        {
            // state.cell_Image.sprite = RankingManager.Instance.GetRanking(index).;
            state.cell_Image.sprite = null;
            state.nameText.text = RankingManager.Instance.GetRanking(index).Nickname;
            state.subText1.text = RankingManager.Instance.GetRanking(index).Grade + " 급";
            state.subText2.text = RankingManager.Instance.GetRanking(index).WinCount+ " 승";
            state.subText3.text = RankingManager.Instance.GetRanking(index).LoseCount+ " 패";
            state.subText4.text = RankingManager.Instance.GetWinRate(index).ToString("F2") + "%";
        }

        else if (state.cellType == CellState.CellType.Record)
        {
            state.cell_Image.sprite = RecordManager.Instance.GetSprite(index);
            state.nameText.text = RecordManager.Instance.GetRecordName(index);
            state.subText1.text = RecordManager.Instance.GetName(index);
            state.subText2.text = RecordManager.Instance.GetDate(index).ToString();

            state.buttonObj.AddComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.ButtonClickSound();//버튼 클릭음
                RecordManager.Instance.RemoveRecord(index);
            });                
            cell.transform.GetChild(0).gameObject.AddComponent<Button>().onClick.AddListener(() => { RecordManager.Instance.RecordReplay(index); });
        }
        return;
    }

    switch (state.cellType)
    {
        case CellState.CellType.Ranking:
            // SafeSetImage(state.cell_Image, RankingManager.Instance.GetSprite(index));
            SafeSetText(state.nameText, RankingManager.Instance.GetRanking(index).Nickname);
            SafeSetText(state.subText1, $"{RankingManager.Instance.GetRanking(index).Grade} 급");
            SafeSetText(state.subText2, $"{RankingManager.Instance.GetRanking(index).WinCount} 승");
            SafeSetText(state.subText3, $"{RankingManager.Instance.GetRanking(index).LoseCount} 패");
            SafeSetText(state.subText4, $"{RankingManager.Instance.GetWinRate(index):F2}%");
            break;

        case CellState.CellType.Record:
            SafeSetImage(state.cell_Image, RecordManager.Instance.GetSprite(index));
            SafeSetText(state.nameText, RecordManager.Instance.GetRecordName(index));
            SafeSetText(state.subText1, RecordManager.Instance.GetName(index));
            SafeSetText(state.subText2, RecordManager.Instance.GetDate(index).ToString());

            AddSafeButton(state.buttonObj, () => RecordManager.Instance.RemoveRecord(index));
            AddSafeButton(cell.transform.GetChild(0).gameObject, () => RecordManager.Instance.RecordReplay(index));
                break;

        case CellState.CellType.Shop:
            SafeSetImage(state.cell_Image, ShopManager.Instance.GetSprite(index));
            SafeSetText(state.nameText, ShopManager.Instance.GetName(index));
            SafeSetText(state.subText1, $"{ShopManager.Instance.GetNum(index)} 코인");
            SafeSetText(state.subText2, $"{ShopManager.Instance.GetPrice(index)} 원");
            SafeSetText(state.subText3, ""); // Shop에서는 안 씀 → 빈 문자열
            SafeSetText(state.subText4, "");

            AddSafeButton(state.buttonObj, () => ShopManager.Instance.BuyCoin(index));

                break;

       default:
            Debug.LogWarning($"[CellInfoUpdate] Unknown cell type: {state.cellType}");
            break;
    }
}

    void SafeSetText(TMP_Text textComponent, string value)
    {
        if (textComponent != null)
            textComponent.text = value;
    }

    void SafeSetImage(Image imageComponent, Sprite sprite)
    {
        if (imageComponent != null)
            imageComponent.sprite = sprite;
    }

    void AddSafeButton(GameObject buttonObj, UnityEngine.Events.UnityAction action)
    {
        if (buttonObj == null) return;

        Button btn = buttonObj.GetComponent<Button>();
        if (btn == null) btn = buttonObj.AddComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }
}