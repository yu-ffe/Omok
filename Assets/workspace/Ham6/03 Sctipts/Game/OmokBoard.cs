using UnityEngine;
using UnityEngine.UI;

public class OmokBoard : MonoBehaviour
{
    [SerializeField] private RectTransform boardImage; // 바둑판 UI 이미지 (현재 크기)
    [SerializeField] private Vector2 originalBoardSize = new Vector2(692, 692); // 원본 바둑판 이미지 크기
    [SerializeField] private float originalPadding = 30f; // 원본 공백 크기
    [SerializeField] private GameObject MakerBlackPrefab; // 흑바둑알 프리팹
    [SerializeField] private GameObject MakerWhitePrefab; // 백바둑알 프리팹
    [SerializeField] private int gridSize = 15; // 격자 수 (15x15)

    private float scaledPadding; // 현재 UI 기준 비례 공백
    private float cellSize; // 현재 UI 기준 한 칸 크기
    private Vector2 startPosition; // 바둑판의 시작 좌표

    void Start()
    {
        CalculateSizes();
        
        // 테스트용 바둑알 배치
        bool isblack = false;
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                PlaceStone(j, i, isblack);
                isblack = !isblack;
            }
        }
        
    }

    //바둑판 크기의 비례한 공백과 시작위치 계산
    void CalculateSizes()
    {
        float currentBoardSize = boardImage.rect.width; // 현재 UI에서 바둑판 이미지 크기
        float scaleFactor = currentBoardSize / originalBoardSize.x; // 원본 대비 현재 배율

        scaledPadding = originalPadding * scaleFactor; // 현재 크기에 맞춰 공백 조정
        float playableSize = currentBoardSize - (scaledPadding * 2); // 공백을 제외한 실제 바둑판 크기
        cellSize = playableSize / (gridSize - 1); // 한 칸 크기 (간격 기준)
        
        // 시작 좌표 설정 (공백을 기준으로 조정)
        startPosition = new Vector2(scaledPadding, -scaledPadding);

        Debug.Log($"현재 바둑판 크기: {currentBoardSize}, 조정된 공백: {scaledPadding}, 한 칸 크기: {cellSize}");
        Debug.Log($"시작 좌표: {startPosition}");
    }

    //바둑알 착수 함수
    public void PlaceStone(int x, int y, bool isBlack)
    {
        Vector2 localPos  = GetLocalPosition(x, y); //바둑알의 배열을 읽고 로컬위치로 바꿔줌
        GameObject stone = Instantiate(isBlack ? MakerBlackPrefab : MakerWhitePrefab, boardImage); //바둑판의 자식으로 바둑알 생성
        
        RectTransform stoneRect = stone.GetComponent<RectTransform>(); //바둑알의 로컬위치
        stoneRect.anchoredPosition = localPos; //바둑알의 로컬위치 조정
        
        float stoneSize = cellSize * 0.85f; // 바둑알 크기(한 칸 크기의 85%)
        stone.GetComponent<RectTransform>().sizeDelta = new Vector2(stoneSize, stoneSize); // 바둑알 크기 조정
    }

    //바둑알의 배열을 읽고 로컬위치로 바꿔주는 함수
    public Vector2 GetLocalPosition(int x, int y)
    {
        float localX  = startPosition.x + (x * cellSize);
        float localY  = startPosition.y - (y * cellSize); // Y 방향은 아래로 감소
        
        Debug.Log($"{x}, {y} 의 좌표: {localX}, {localY}");

        return new Vector2(localX, localY);
    }
}
