using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using workspace.Ham6._03_Sctipts.Game;

public class OmokBoard : MonoBehaviour, IPointerMoveHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform boardImage; // 바둑판 UI 이미지 (현재 크기)
    [SerializeField] private Vector2 originalBoardSize = new Vector2(692, 692); // 원본 바둑판 이미지 크기
    [SerializeField] private float originalPadding = 30f; // 원본 공백 크기
    [SerializeField] private GameObject MarkerBlackPrefab; // 흑바둑알 프리팹
    [SerializeField] private GameObject MarkerWhitePrefab; // 백바둑알 프리팹
    [SerializeField] private GameObject MarkerLastPrefab; // 마지막수 프리팹
    [SerializeField] private GameObject MarkerXPrefab; // 금지 프리팹
    [SerializeField] private GameObject MarkerPositionSelecorPrefab; // 착수예정 프리팹
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
    
    public void ShowLastStone()
    {
        // 마지막마커 표시 (기존 마지막마커 오브젝트를 지우고 다시 생성)
        GameObject marker_Last = GameObject.Find("marker_Last");
        if (marker_Last) Destroy(marker_Last);
        
        Debug.Log($"마지막 돌 위치 {GameManager.Instance.GameLogicInstance.moveList.Last().x},{GameManager.Instance.GameLogicInstance.moveList.Last().y}");
        
        PlaceStone(Constants.PlayerType.PlayerA, Constants.StoneType.Last, GameManager.Instance.GameLogicInstance.moveList.Last().x ,GameManager.Instance.GameLogicInstance.moveList.Last().y);
    }

    //바둑판 크기의 비례한 공백과 시작위치 계산
    void CalculateSizes()
    {
        float currentBoardSize = boardImage.rect.width; // 현재 UI에서 바둑판 이미지 크기
        float scaleFactor = currentBoardSize / originalBoardSize.x; // 원본 대비 현재 배율

        scaledPadding = originalPadding * scaleFactor; // 현재 크기에 맞춰 공백 조정
        float playableSize = currentBoardSize - (scaledPadding * 2); // 공백을 제외한 실제 바둑판 크기
        cellSize = playableSize / (gridSize - 1); // 한 칸 크기 (간격 기준)
        
        // 바둑알 시작 좌표를 우하단으로 설정 (공백을 기준으로 조정)
        startPosition = new Vector2(scaledPadding, scaledPadding);
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

        GameObject stone = null;

        // 2. StoneType에 따라 적절한 프리팹 생성
        switch (stoneType)
        {
            case Constants.StoneType.Normal:
            case Constants.StoneType.Hint:
            {
                // 플레이어에 따라 검은 돌, 흰 돌 프리팹 선택
                GameObject prefab = (playerType == Constants.PlayerType.PlayerA) ? MarkerBlackPrefab : MarkerWhitePrefab;
                stone = Instantiate(prefab, boardImage);

                if (stoneType == Constants.StoneType.Hint)
                {
                    // 힌트 돌의 이름 지정 및 반투명 처리
                    stone.name = "HintStone";
                    Image stoneImage = stone.GetComponent<Image>();
                    if (stoneImage)
                    {
                        stoneImage.color = new Color(1f, 1f, 1f, 0.6f);
                    }
                }
                else
                {
                    // 일반 돌인 경우 로그 출력
                    Debug.Log($"{x}, {y}에 착수");
                }
                break;
            }
            case Constants.StoneType.Last:
                stone = Instantiate(MarkerLastPrefab, boardImage);
                stone.name = "marker_Last";
                break;
            case Constants.StoneType.XMark:
                stone = Instantiate(MarkerXPrefab, boardImage);
                stone.name = "marker_X";
                break;
            case Constants.StoneType.PositionSelecor:
                stone = Instantiate(MarkerPositionSelecorPrefab, boardImage);
                stone.name = "marker_PositionSelecor";
                break;
            default:
                Debug.LogError($"알 수 없는 StoneType: {stoneType}");
                return;
        }

        // 3. 생성된 돌의 위치와 크기 설정
        RectTransform stoneRect = stone.GetComponent<RectTransform>();
        
        if (stoneRect)
        {
            stoneRect.anchoredPosition = localPos;
            float stoneSize = cellSize * 0.85f;
            stoneRect.sizeDelta = new Vector2(stoneSize, stoneSize);
        }
        else
        {
            Debug.LogWarning("생성된 돌에 RectTransform 컴포넌트가 없습니다.");
        }
    }
    
    //바둑알의 배열을 읽고 지정된 로컬좌표로 바꿔주는 함수
    public Vector2 GetLocalPosition(int x, int y)
    {
        float localX  = startPosition.x + (x * cellSize);
        float localY  = startPosition.y + (y * cellSize); // Y 방향은 위로

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

        float leftBottomX = localPoint.x + (rectTransform.rect.width * 0.5f);
        float leftBottomY = localPoint.y + (rectTransform.rect.height * 0.5f);

        Vector2 leftBottomPoint = new Vector2(leftBottomX, leftBottomY);
        
        boardCoord = GetCoord(leftBottomPoint);
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
