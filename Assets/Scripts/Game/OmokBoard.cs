using System;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Commons.Models;
using Commons.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
    
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
    
    private GameObject MarkerBlackHint; // 흑바둑알 게임 오브젝트
    private GameObject MarkerWhiteHint; // 백바둑알 게임 오브젝트
    private GameObject MarkerLast; // 마지막수 게임 오브젝트
    private GameObject MarkerX; // 금지 게임 오브젝트
    private GameObject MarkerPosition; // 착수예정 게임 오브젝트

    private RectTransform MarkerBlackHintRt; // 흑바둑알 게임 오브젝트의 RectTransform
    private RectTransform MarkerWhiteHintRt; // 백바둑알 게임 오브젝트의 RectTransform
    private RectTransform MarkerLastRt; // 마지막수 게임 오브젝트의 RectTransform
    private RectTransform MarkerXRt; // 금지 게임 오브젝트의 RectTransform
    private RectTransform MarkerPositionRt; // 착수예정 게임 오브젝트의 RectTransform
    
    
    List<Vector2Int> xMarkerCoords = new();
    [SerializeField]private List<(GameObject marker, RectTransform rectTransform)> markerList = new List<(GameObject marker, RectTransform rectTransform)>();
    private int nextMarkerIndex = 0;
    
    private GameObject hintStone; // 투명알 프리팹
    public int gridSize = 15; // 격자 수 (15x15)

    private float scaledPadding; // 현재 UI 기준 비례 공백
    private float cellSize; // 현재 UI 기준 한 칸 크기
    private Vector2 startPosition; // 바둑판의 시작 좌표
    
    private bool inBoard = false; // 보드 안에 있는지 여부
    
    public RectTransform rectTransform;
    
    private Vector2 localPoint;     //마우스의 좌표를 UI좌표로 바꾼 값
    private Vector2Int boardCoord;  //UI좌표를 배열로 바꾼 값
    
    private Vector2Int selectedBoardCoord;  //마우스를 땟을 때 UI좌표를 배열로 바꾼 값
    
    bool ismarker_Last = false;
    GameObject marker_Last = null;
    GameEnums.GameType gameType;
    
    List<GameObject> stoneObjects = new();
    
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
    private void Awake()
    {
        GameManager.Instance.omokBoard = this;
        Debug.Log($"{GameManager.Instance.omokBoard} 게임보드");
    }
    
    void Start()
    {
        CalculateSizes();
        InitializeGameObjects();
        gameType = GameManager.Instance.GetGameType();
    }
    
    //마크 오브젝트를 할당하는 함수
    void InitializeGameObjects()
    {
        MarkerBlackHint = CreateDisabledInstance(MarkerBlackPrefab); // 흑바둑알 게임 오브젝트
        
        MarkerWhiteHint = CreateDisabledInstance(MarkerWhitePrefab); // 백바둑알 게임 오브젝트

        MarkerLast = CreateDisabledInstance(MarkerLastPrefab); // 마지막수마크 게임 오브젝트

        MarkerX = CreateDisabledInstance(MarkerXPrefab); // 금지마크 게임 오브젝트

        MarkerPosition = CreateDisabledInstance(MarkerPositionSelecorPrefab); // 착수예정마크 게임 오브젝트
    }
    
    //바둑판에 사용될 마크 오브젝트를 생성하는 함수
    private GameObject CreateDisabledInstance(GameObject prefab)
    {
        // boardImage의 자식으로 prefab을 생성하고 비활성화
        GameObject instance = Instantiate(prefab, boardImage);
        instance.SetActive(false);

        // 인스턴스의 RectTransform을 미리 캐싱
        RectTransform rt = instance.GetComponent<RectTransform>();

        // 기본 돌 크기를 계산 (cellSize는 클래스 멤버 변수)
        float stoneSize = cellSize * 0.85f;

        // prefab 종류에 따라 추가 설정 진행
        if (prefab == MarkerBlackPrefab || prefab == MarkerWhitePrefab)
        {
            // 흑 또는 백 프리팹인 경우, Image 컴포넌트를 찾아 반투명 색상 적용
            Image stoneImage = instance.GetComponent<Image>();
            if (stoneImage != null)
            {
                stoneImage.color = new Color(1f, 1f, 1f, 0.6f);
            }
            // 각 프리팹에 해당하는 전역 RectTransform 변수에 캐싱
            if (prefab == MarkerBlackPrefab)
            {
                MarkerBlackHintRt = rt;
            }
            else // MarkerWhitePrefab
            {
                MarkerWhiteHintRt = rt;
            }
        }
        else if (prefab == MarkerLastPrefab)
        {
            // 마지막 수 프리팹: 돌 크기를 1/3로 줄임
            stoneSize /= 3f;
            MarkerLastRt = rt;
        }
        else if (prefab == MarkerXPrefab)
        {
            // 금지 마크 프리팹: 돌 크기를 0.5배로 조정
            stoneSize *= 0.7f;
            MarkerXRt = rt;
        }
        else if (prefab == MarkerPositionSelecorPrefab)
        {
            // 착수 예정 마크 프리팹: 별도 처리 없이 캐싱
            MarkerPositionRt = rt;
        }
        else
        {
            Debug.LogWarning("알 수 없는 prefab이 전달되었습니다.");
        }

        // 캐싱된 RectTransform을 통해 돌의 크기를 설정
        rt.sizeDelta = new Vector2(stoneSize, stoneSize);

        return instance;
    }


    void Update()
    {
        ShowHintStone(boardCoord);
        // 현재 좌표에 미리보기 돌을 표시 (현재까지 돌 개수를 기준으로 색상을 결정)
    }
    
    //미리보기 마커 생성함수
    private void ShowHintStone(Vector2Int coord)
    {
        if (!GameManager.Instance || GameManager.Instance.gameLogic == null || GameEnums.GameType.Record == gameType)
        {
            return;
        }
        
        GameEnums.PlayerType currentPlayer = GameManager.Instance.gameLogic.GetCurrentPlayerType();
        GameObject hintMarker = MarkerBlackHint;
        RectTransform rt = MarkerBlackHintRt;
            
        if (currentPlayer == GameEnums.PlayerType.PlayerA)
        {
            hintMarker = MarkerBlackHint;
            rt = MarkerBlackHintRt;
        }
        else
        {
            hintMarker = MarkerWhiteHint;
            rt = MarkerWhiteHintRt;
        }
        
        if (GameManager.Instance.GetGameType() == GameEnums.GameType.SinglePlayer) {
            if (GameManager.Instance.GameLogicInstance.GetCurrentPlayerType() == GameEnums.PlayerType.None) {
                return; // AI타입이 NONE이라서 일단 그대로 넣어둠
            }
        } // 아래 코드와 중첩되는것같지만 일단 그대로 둠
        
        //돌이 비워있고, 마우스가 보드안에 있으면
        bool isValidGameType = GameManager.Instance.GetGameType() == GameEnums.GameType.SinglePlayer ||
                               GameManager.Instance.GetGameType() == GameEnums.GameType.MultiPlayer;

        bool isCurrentPlayerA = GameManager.Instance.gameLogic.GetCurrentPlayerType() == GameEnums.PlayerType.PlayerA;
        
        bool isCellValid = GameManager.Instance.gameLogic.IsCellEmpty(coord.x, coord.y) && inBoard;

        if (isValidGameType && isCurrentPlayerA || !isValidGameType)
        {
            hintMarker.SetActive(isCellValid);
            if (isCellValid)
            {
                ShowMarker(hintMarker, rt, coord);
            }
        }
        else
        {
            hintMarker.SetActive(false);
        }
    }
    
    //마지막에 착수된 마커 표시하기 위한 함수
    public void ShowLastStone()
    {
        var lastMove = GameManager.Instance.gameLogic.moveList.Last();
        Vector2Int localPos = new Vector2Int(lastMove.x, lastMove.y);
        
        ShowMarker(MarkerLast,MarkerLastRt, localPos);
        MarkerLast.transform.SetAsLastSibling();
        //Debug.Log($"{lastMove.x},{lastMove.y}에 라스트 마크 생성");
    }

    public void ShowXMarker()
    {
        RuleCheckers ruleCheckers = new RuleCheckers();
        ruleCheckers.Initialize(GameManager.Instance.gameLogic.GetBoard());

        xMarkerCoords = ruleCheckers.CheckAllBoard();
        
        for (int i = 0; i < xMarkerCoords.Count; i++)
        {
            Vector2Int coord = xMarkerCoords[i];
            Vector2 localPos  = GetLocalPosition(coord.x, coord.y);
            
            GameObject marker;
            RectTransform rt;

            // 재사용 가능한 마커가 있으면 사용
            if (i < markerList.Count)
            {
                marker = markerList[i].marker;
                rt = markerList[i].rectTransform;
            }
            // 없으면 새로 생성 후 리스트에 추가
            else
            {
                marker = Instantiate(MarkerX, boardImage);
                rt = marker.GetComponent<RectTransform>();
                if (rt == null)
                {
                    Debug.LogError("생성된 마커에 RectTransform 컴포넌트가 없습니다.");
                    continue;
                }
                markerList.Add((marker, rt));
            }

            // 마커 활성화 및 anchoredPosition 업데이트
            GameManager.Instance.gameLogic.GetBoard()[coord.x,coord.y] = GameEnums.PlayerType.PlayerX;
            
            marker.SetActive(true);
            rt.anchoredPosition = new Vector2(localPos.x, localPos.y);
        }
    }
    
    public void RemoveXmarker()
    {
        foreach (var markerData in markerList)
        {
            if (markerData.marker != null)
            {
                markerData.marker.SetActive(false);
            }
        }
        // 재활성화를 위한 인덱스 초기화
        nextMarkerIndex = 0;
        
        for (int i = 0; i < xMarkerCoords.Count; i++)
        {
            Vector2Int coord = xMarkerCoords[i];
            GameManager.Instance.gameLogic.GetBoard()[coord.x,coord.y] = GameEnums.PlayerType.None;
        }
    }

    //바둑판 위의 마커들을 활성화 하고 위치를 바꿔주는 함수
    public void ShowMarker(GameObject mark,RectTransform rt, Vector2Int coord)
    {
        Vector2 localPos  = GetLocalPosition(coord.x, coord.y);
        
        mark.SetActive(true);
        
        rt.anchoredPosition = localPos;
    }
    
    // AI가 백인 경우에만 동작, 흑백 랜덤 시작으로 개선시에는 수정 필요
    public void AIWhiteShowMarker((int, int) pos) {
        // Debug.Log($"AI 착수 위치: {pos.Item1}, {pos.Item2}");
        MarkerWhiteHint.SetActive(true);
        // Debug.Log($"AI 착수 위치: {pos.Item1}, {pos.Item2}");
        MarkerWhiteHintRt.anchoredPosition = GetLocalPosition(pos.Item1, pos.Item2);
    }
    
    public void AIWhiteHideMarker() {
        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            // SetActive 또는 다른 Unity API 호출
            MarkerWhiteHint.SetActive(false); // 예시로 SetActive 호출
        });
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
    public void PlaceStone(GameEnums.PlayerType playerType, int x, int y)
    {
        
        Vector2 localPos  = GetLocalPosition(x, y); //바둑알의 배열을 읽고 바둑판로컬위치로 바꿔줌

        GameObject stone = Instantiate(playerType == GameEnums.PlayerType.PlayerA ? MarkerBlackPrefab : MarkerWhitePrefab, boardImage);
        // 3. 생성된 돌의 위치와 크기 설정
        float stoneSize = cellSize * 0.85f;

        if (GameEnums.GameType.Record == gameType)
        {
            GameObject stoneNum = new GameObject("StoneNum");

            stoneNum.transform.SetParent(stone.transform);

            TextMeshProUGUI tmpText = stoneNum.AddComponent<TextMeshProUGUI>();
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.fontSize = 18;
            tmpText.fontStyle = FontStyles.Bold;
            
            RectTransform rect = tmpText.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            
            int siblingIndex = boardImage.childCount - 5;
            tmpText.text = "" + siblingIndex;

            if (siblingIndex % 2 == 0)
            {
                tmpText.color = Color.black;
            }
            else
            {
                tmpText.color = Color.white;
            }
        }
        
        RectTransform stoneRect = stone.GetComponent<RectTransform>();
        stoneRect.anchoredPosition = localPos;
        stoneRect.sizeDelta = new Vector2(stoneSize, stoneSize);
    }

    public void RemoveStone()
    {
        if (boardImage.childCount > 0)
        {
            Transform lastChild = boardImage.GetChild(boardImage.childCount - 1);
            Destroy(lastChild.gameObject);
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
        if (GameEnums.GameType.Record == gameType)
        {
            return;
        }
        
        if (GameManager.Instance.gameLogic.IsCellEmpty(boardCoord.x, boardCoord.y))
        {
            selectedBoardCoord = boardCoord;
            //선택 마크를 출력
            ShowMarker(MarkerPosition, MarkerPositionRt, selectedBoardCoord);
        }
    }
    
    //착수 버튼에 할당할 함수
    public void OnPointerUpMarkerPositionSelecor()
    {
        SoundManager.Instance.PlayPlacingSound(); // 바둑돌 놓는 효과음

        if (OnOnGridClickedDelegate != null)
            if (OnOnGridClickedDelegate != null && MarkerPosition.activeSelf)
            {
                OnOnGridClickedDelegate.Invoke(selectedBoardCoord.x, selectedBoardCoord.y);
            }
        else
        {
            Debug.Log($"델리게이트 없음");
        }
    }
    
    public void OnPointerUpCurrentPlayerDefeat()
    {
        GameManager.Instance.gameLogic.HandleCurrentPlayerDefeat(GameManager.Instance.gameLogic.GetCurrentPlayerType());
    }
}