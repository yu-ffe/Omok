using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSet : MonoBehaviour
{
    // �� ��ũ��Ʈ�� On ���ִ� �Ŵ����� ������ ��
    [Header("�ʼ� �Ҵ�")]
    [SerializeField] GameObject cellPrefab; // content �� ������
    RectTransform cellRect;
    float cellWidth;  // �� ���� �ʺ�
    float cellHeight; // �� ���� ����

    [SerializeField] GameObject content; // content (�θ� ������Ʈ)
    [SerializeField] RectTransform contentRectTransform;

    [SerializeField] ScrollRect scrollRect;

    List<GameObject> pool = new List<GameObject>(); // ������Ʈ Ǯ�� ����Ʈ

    [Header("�ʼ� �Է�")]
    [SerializeField, Tooltip("�� ���� ����")] float spacing;     // �� ���� ����
    [SerializeField, Tooltip("��ü �� ��")] int totalCells;
    [SerializeField, Tooltip("������ �� ��(���� ����)(����:���̴� �� + (���� �� ���� * 3))")] int createCellCount;
    [SerializeField, Tooltip("���� �� ����")] int cellRowCount;


    private float contentWidth; // ������ �ʺ�
    private float contentHeight; // ������ ����
    int lastStartIndex;


    [Header("�߰� �ʿ� ����")]
    bool needMore;
    // [SerializeField] Sprite normalClear, perfectClear; // Ŭ���� ���� ������

    private void Start()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);

        CellInfoSet();
    }

    void CellInfoSet()
    {
        cellRect = cellPrefab.GetComponent<RectTransform>();

        // �� ����
        cellWidth = cellRect.sizeDelta.x;
        cellHeight = cellRect.sizeDelta.y;

        // �� �»�� ����
        cellRect.anchorMin = new Vector2(0, 1);
        cellRect.anchorMax = new Vector2(0, 1);
        cellRect.pivot = new Vector2(0, 1);

        // �ڵ� ����(�ν����� �Ҵ� ����)
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

        ContentSizeSet(); // ��ũ�� ������ ������
    }

    void ContentSizeSet()
    {
        contentWidth = (cellWidth + spacing) * cellRowCount - spacing; // ���� ũ�� ��� (������ �� spacing ����)

        int totalRows = Mathf.CeilToInt((float)totalCells / cellRowCount);
        contentHeight = (cellHeight + spacing) * totalRows - spacing; // ������ �� �Ʒ��ʿ� ���ʿ��� spacing ����

        // content ��� ����
        contentRectTransform.anchorMin = new Vector2(0.5f, 1);
        contentRectTransform.anchorMax = new Vector2(0.5f, 1);
        contentRectTransform.pivot = new Vector2(0.5f, 1);

        contentRectTransform.sizeDelta = new Vector2(contentWidth, contentHeight);
        contentRectTransform.anchoredPosition = new Vector2(0, 0);


        InitPool();
    }

    void InitPool()
    {
        for (int i = 0; i < createCellCount; i++) // �� 1�� �Ʒ� 2�� ����
        {
            GameObject stageObj = Instantiate(cellPrefab, content.transform);
            stageObj.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
            pool.Add(stageObj);
        }

        CellPositionSet(0);
    }

    void CellPositionSet(int startRow)
    {
        // ���� ��ġ ����

        for (int i = 0; i < pool.Count; i++)
        {
            int row = (i + startRow * cellRowCount) / cellRowCount; // ���� ���� �������� �� ���
            int col = (i + startRow * cellRowCount) % cellRowCount; // �� ���

            float xPos = col * (cellWidth + spacing);   // ���� ��ġ
            float yPos = -row * (cellHeight + spacing); // ���� ��ġ (�� �� �Ʒ�)

            RectTransform rect = pool[i].GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(xPos, yPos);
            pool[i].SetActive(true); // �ʱ� ȭ�鿡 ���̵��� Ȱ��ȭ
        }
    }


    public void StageSelectPopSet() // ���� �ܺ� ȣ��
    {
        scrollRect.verticalNormalizedPosition = 1f;
        UpdateStages(GetStartIndex());
    }


    private void OnScroll(Vector2 scrollPos)
    {
        var startIndex = GetStartIndex();

        if (startIndex > lastStartIndex) // �Ʒ��� ����
        {
            for (int i = 0; i < cellRowCount; i++)
            {
                // Ǯ ���� 1���� �� ������ �̵�
                GameObject stageObj = pool[0];
                pool.RemoveAt(0);
                pool.Add(stageObj);
            }

            CellPositionSet(startIndex / cellRowCount);

            UpdateStages(startIndex); // �� ��ġ�� ���� �߰� ����
        }

        else if (startIndex < lastStartIndex) // ���� ����
        {
            // Ǯ �� 1���� �� ���� �̵�
            for (int i = 0; i < cellRowCount; i++)
            {
                GameObject stageObj = pool[pool.Count - 1];
                pool.RemoveAt(pool.Count - 1);
                pool.Insert(0, stageObj);
            }

            CellPositionSet(startIndex / cellRowCount);

            UpdateStages(startIndex); // �� ��ġ�� ���� �߰� ����
        }

        lastStartIndex = startIndex;
    }


    int GetStartIndex() // ���� �� ���� �ε��� ���
    {
        // ���̴� ������ Rect
        Rect visibleRect = new Rect(
            scrollRect.content.anchoredPosition.x,
            scrollRect.content.anchoredPosition.y,
            contentRectTransform.rect.width,
            contentRectTransform.rect.height
        );



        // �� ���̿� ������ �ݿ��Ͽ� ���̴� �������� �� ��° ������ �����ϴ��� ���
        float totalCellHeight = cellHeight + spacing; // �� ���� + ����
        var startIndex = Mathf.FloorToInt(visibleRect.y / totalCellHeight); // ��ü �� ���� �������� ���� �ε��� ���



        // ���� ���� �ε����� stagesPerRow�� �°� ����
        startIndex = startIndex * cellRowCount;

        // �� ���� �ƴϸ� ���� ���� 1���� �ݿ� (���� �ε��� ����)
        startIndex = Mathf.Max(0, startIndex - cellRowCount);

        // �� �Ʒ��� �ƴϸ� �Ʒ��� ���� 1���� �ݿ�
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

            stageObj.SetActive(true); // ���� ���������� Ȱ��ȭ

            CellInfoUpdate(stageObj, stageLevel); // ������ ������Ʈ       
        }
    }








    

    void CellInfoUpdate(GameObject cell, int cellNum)
    {
        CellState state = cell.GetComponent<CellState>();

        if (state.cellType == CellState.CellType.Ranking)
        {
            state.cell_Image.sprite = RankingManager.Instance.GetSprite();
            state.nameText.text = RankingManager.Instance.GetName();
            state.subText1.text = RankingManager.Instance.GetGrade().ToString();
            state.subText2.text = RankingManager.Instance.GetWin().ToString();
            state.subText3.text = RankingManager.Instance.GetLose().ToString();
        }

        else if (state.cellType == CellState.CellType.Record)
        {
            state.cell_Image.sprite = RecordManager.Instance.GetSprite();
            state.nameText.text = RecordManager.Instance.GetRecordName();
            state.subText1.text = RecordManager.Instance.GetName();
            state.subText2.text = RecordManager.Instance.GetDate().ToString();
            // TODO �⺸ ����
            state.deleteButtonObj.AddComponent<Button>().onClick.AddListener(()=> { Debug.Log("�� ��ư Ŭ�� �� �⺸ ����(RecordManager)"); });
        }

        else if (state.cellType == CellState.CellType.Shop)
        {
            state.cell_Image.sprite = ShopManager.Instance.GetSprite();
            state.nameText.text = ShopManager.Instance.GetName();
            state.subText1.text = ShopManager.Instance.GetPrice().ToString();
        }
        // UI ������Ʈ
      


        // �̺�Ʈ ������ ������Ʈ
        // state.selectButton.onClick.RemoveAllListeners();

        // int currentStageIndex = stageLevel;  // Ŭ���� ���� �ذ��� ���� ���� �ε��� ����
        // state.selectButton.onClick.AddListener(() => OnStageSelected(currentStageIndex)); // Ŭ���� ���� �ذ� �ʿ�

        // Ŭ���� ���� ����
        // int stageStars = KeyController.GetStageStars(cellNum);
        // int stageScore = KeyController.GetStageScore(cellNum);
        // SetupStageState(state, stageStars, stageScore, cellNum, normalClear, perfectClear);

    }


    private void OnStageSelected(int level)
    {
        Debug.Log($"�������� {level} ����");
    }
    

}
