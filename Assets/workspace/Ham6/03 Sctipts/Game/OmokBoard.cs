using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using workspace.Ham6._03_Sctipts.Game;

public class OmokBoard : MonoBehaviour, IPointerMoveHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform boardImage; // 바둑판 UI 이미지 (현재 크기)
    [SerializeField] private Vector2 originalBoardSize = new Vector2(692, 692); // 원본 바둑판 이미지 크기
    [SerializeField] private float originalPadding = 30f; // 원본 공백 크기
    [SerializeField] private GameObject MakerBlackPrefab; // 흑바둑알 프리팹
    [SerializeField] private GameObject MakerWhitePrefab; // 백바둑알 프리팹
    private GameObject hintStone; // 투명알 프리팹
    public int gridSize = 15; // 격자 수 (15x15)

    private float scaledPadding; // 현재 UI 기준 비례 공백
    private float cellSize; // 현재 UI 기준 한 칸 크기
    private Vector2 startPosition; // 바둑판의 시작 좌표
    
    private bool inBoard = false; // 보드 안에 있는지 여부
    
    public RectTransform rectTransform;
    
    private Vector2 localPoint;     //마우스의 좌표를 UI좌표로 바꾼 값
    private Vector2Int boardCoord;  //UI좌표를 배열로 바꾼 값
    
    public delegate void OnGridClicked(int row, int col);
    public OnGridClicked OnOnGridClickedDelegate;

    /*public void InitStones()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].InitMarker(i, blockIndex =>
            {
                var clickedRow = blockIndex / 3;
                var clickedCol = blockIndex % 3;
                
                OnOnGridClickedDelegate?.Invoke(clickedRow, clickedCol);
            });
        }
    }*/

    void Start()
    {
        CalculateSizes();
    }

    void Update()
    {
        // 만약 포인터가 보드 내에 있고, 현재 좌표에 돌이 아직 배치되지 않았다면
        if (inBoard)
        {
            // 현재 좌표에 미리보기 돌을 표시 (현재까지 돌 개수를 기준으로 색상을 결정)
            ShowHintStone(boardCoord);
        }
    }
    
    //미리보기 돌 생성함수
    private void ShowHintStone(Vector2Int coord)
    {
        Constants.PlayerType currentPlayer = GameManager.Instance.GameLogicInstance.GetCurrentPlayerType();
        
        // 미리보기용 투명한 돌 표시 (기존 미리보기용 오브젝트를 지우고 다시 생성)
        hintStone = GameObject.Find("HintStone");
        if (hintStone) Destroy(hintStone);
        
        if (!GameManager.Instance.GameLogicInstance.IsCellEmpty(coord.x, coord.y))
        {
            // 돌을 놓을 수 없는 자리이면 힌트 돌 표시하지 않음
            Destroy(hintStone);
            return;
        }
        
        PlaceStone(currentPlayer, Constants.StoneType.Hint, coord.x ,coord.y);
    }

    //바둑판 크기의 비례한 공백과 시작위치 계산
    void CalculateSizes()
    {
        float currentBoardSize = boardImage.rect.width; // 현재 UI에서 바둑판 이미지 크기
        float scaleFactor = currentBoardSize / originalBoardSize.x; // 원본 대비 현재 배율

        scaledPadding = originalPadding * scaleFactor; // 현재 크기에 맞춰 공백 조정
        float playableSize = currentBoardSize - (scaledPadding * 2); // 공백을 제외한 실제 바둑판 크기
        cellSize = playableSize / (gridSize - 1); // 한 칸 크기 (간격 기준)
        
        // 바둑알 시작 좌표 설정 (공백을 기준으로 조정)
        startPosition = new Vector2(scaledPadding, -scaledPadding);
    }

    //마우스로 입력 받은 보드 로컬좌표를 배열로 바꾸는 함수
    private Vector2Int GetCoord(Vector2 localPoint)
    {
        // 보드의 X좌표 계산: 오프셋을 빼고 격자 간격으로 나눈 후 반올림
        int coordX = Mathf.RoundToInt((localPoint.x - scaledPadding) / cellSize);
        // 보드의 Y좌표 계산: 오프셋을 빼고 격자 간격으로 나눈 후 반올림
        int coordY = Mathf.RoundToInt((localPoint.y - scaledPadding) / cellSize);
        
        // 계산된 X좌표가 0과 BOARD_SIZE 사이에 있도록 제한
        coordX = Mathf.Clamp(coordX, 0, gridSize -1);
        // 계산된 Y좌표가 0과 BOARD_SIZE 사이에 있도록 제한
        coordY = Mathf.Clamp(coordY, 0, gridSize -1 );
        
        return new Vector2Int(coordX, coordY);     // 보드 좌표(Vector2Int)를 반환
    }
    
    //바둑알을 바둑판로컬위치에 착수하는 함수
    public void PlaceStone(Constants.PlayerType playerType,Constants.StoneType stoneType, int x, int y)
    {
        Vector2 localPos  = GetLocalPosition(x, y); //바둑알의 배열을 읽고 바둑판로컬위치로 바꿔줌
        GameObject stone = Instantiate(playerType == Constants.PlayerType.PlayerA ? MakerBlackPrefab : MakerWhitePrefab, boardImage); //바둑판의 자식으로 바둑알 생성

        if (stoneType == Constants.StoneType.Hint)
        {
            stone.name = "HintStone";
        
            Image targetImage = stone.GetComponent<Image>();
            targetImage.color = new Color(1, 1, 1, 0.6f); // 반투명 설정
        }
        
        RectTransform stoneRect = stone.GetComponent<RectTransform>(); //바둑알의 바둑판로컬위치
        stoneRect.anchoredPosition = localPos; //바둑알의 바둑판로컬위치 조정
        
        float stoneSize = cellSize * 0.85f; // 바둑알 크기(한 칸 크기의 85%)
        stone.GetComponent<RectTransform>().sizeDelta = new Vector2(stoneSize, stoneSize); // 바둑알 크기 조정
    }
    
    //바둑알의 배열을 읽고 지정된 로컬좌표로 바꿔주는 함수
    public Vector2 GetLocalPosition(int x, int y)
    {
        float localX  = startPosition.x + (x * cellSize);
        float localY  = startPosition.y - (y * cellSize); // Y 방향은 아래로 감소

        return new Vector2(localX, localY);
    }
    
    // 보드에서 포인터가 나갔을 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        inBoard = false;
        localPoint = Vector2.zero;
        hintStone = GameObject.Find("HintStone");
        Destroy(hintStone);
    }
    
    // 보드에서 포인터가 움직이고 있을 때 실행
    public void OnPointerMove(PointerEventData eventData)
    {
        inBoard = true;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, // 변환할 RectTransform
            eventData.position, // 마우스 클릭 위치 (스크린 좌표)
            eventData.pressEventCamera, // 현재 UI를 렌더링하는 카메라
            out localPoint // 변환된 로컬 좌표 저장
        );

        float leftTopX = localPoint.x + (rectTransform.rect.width * 0.5f);
        float leftTopY = (rectTransform.rect.height * 0.5f) - localPoint.y;

        Vector2 leftTopPoint = new Vector2(leftTopX, leftTopY);
        
        boardCoord = GetCoord(leftTopPoint);
    }
    
    // UI에 마우스를 눌렀을 때 실행
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
    
    // UI에 마우스를 땟을 때 실행
    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnOnGridClickedDelegate != null)
        {
            OnOnGridClickedDelegate.Invoke(boardCoord.x, boardCoord.y);
        }
        else
        {
            Debug.Log($"델리게이트 없음");
        }
        
        //PlaceStone(Constants.PlayerType.PlayerA,Constants.StoneType.Normal, boardCoord.x, boardCoord.y);
        //Debug.Log($"UI 내부 클릭 위치: x: {boardCoord.x},y : {boardCoord.y}");
    }
}
