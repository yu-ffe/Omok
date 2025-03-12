using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmokBoard : MonoBehaviour
{
    public RectTransform boardImage; // 바둑판 이미지 (UI 기준)
    public int gridSize = 15; // 격자 수 (14x14)
    public float padding = 30f; // 바둑판 상하좌우 공백

    void Start()
    {
        // 테스트용으로 (0,0)과 (13,13) 좌표 변환 출력
        Debug.Log(GetWorldPosition(0, 0));
        Debug.Log(GetWorldPosition(13, 13));
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        float boardSize = boardImage.rect.width; // 바둑판 이미지 크기
        float playableSize = boardSize - (padding * 2); // 공백을 제외한 실제 바둑판 크기
        float cellSize = playableSize / gridSize; // 한 칸의 크기

        float startX = boardImage.position.x - (playableSize / 2);
        float startY = boardImage.position.y + (playableSize / 2);

        float worldX = startX + (x * cellSize) + (cellSize / 2);
        float worldY = startY - (y * cellSize) - (cellSize / 2);

        return new Vector2(worldX, worldY);
    }
}
